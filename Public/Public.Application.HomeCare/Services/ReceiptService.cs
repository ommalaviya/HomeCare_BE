using iTextSharp.text;
using iTextSharp.text.pdf;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Extensions;
using Shared.HomeCare.Resources;
using System.Security.Claims;   

namespace Public.Application.HomeCare.Services
{
    public class ReceiptService(
        IBookingRepository bookingRepository,
        ClaimsPrincipal principal) : IReceiptService
    {
        private static readonly BaseColor PrimaryColor = new(67, 56, 202);    
        private static readonly BaseColor AccentColor  = new(99, 102, 241);     
        private static readonly BaseColor LightGray    = new(248, 248, 252);
        private static readonly BaseColor BorderGray   = new(226, 232, 240);
        private static readonly BaseColor TextDark     = new(30, 30, 46);
        private static readonly BaseColor TextMuted    = new(100, 116, 139);
        private static readonly BaseColor GreenColor   = new(22, 163, 74);

        private static readonly Font FontH1     = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, PrimaryColor);
        private static readonly Font FontBody   = FontFactory.GetFont(FontFactory.HELVETICA,      10, TextDark);
        private static readonly Font FontMuted  = FontFactory.GetFont(FontFactory.HELVETICA,       9, TextMuted);
        private static readonly Font FontBold   = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, TextDark);
        private static readonly Font FontStatus = FontFactory.GetFont(FontFactory.HELVETICA_BOLD,  9, BaseColor.WHITE);

        private int CurrentUserId => principal.GetUserId();

        public async Task<byte[]> GenerateBookingInvoiceAsync(int bookingId)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Booking));

            if (booking.UserId != CurrentUserId)
                throw new UnauthorizedAccessException(Messages.Unauthorized);

            using var ms  = new MemoryStream();
            using var doc = new Document(PageSize.A4, 48, 48, 40, 40); 
            var writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;

            doc.Open();

            AddHeader(doc, booking.Id);

            AddBookingInfoSection(doc, booking);

            AddServiceSection(doc, booking);

            if (booking.AssignedPartner is not null)
                AddPartnerSection(doc, booking.AssignedPartner.FullName);

            AddAddressSection(doc, booking);

            AddPaymentSummary(doc, booking);

            AddFooter(doc);

            doc.Close();
            ms.Position = 0;
            return ms.ToArray();
        }

        private static void AddHeader(Document doc, int bookingId)
        {
            var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 60, 40 });
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            var logoCell = new PdfPCell
            {
                Border            = Rectangle.NO_BORDER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingBottom     = 6
            };
            logoCell.AddElement(new Paragraph("HomeCare", FontH1));
            var tagline = new Paragraph("Professional Home Services", FontMuted) { SpacingBefore = 2 };
            logoCell.AddElement(tagline);
            headerTable.AddCell(logoCell);

            var metaCell = new PdfPCell
            {
                Border              = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment   = Element.ALIGN_MIDDLE,
                PaddingBottom       = 6
            };
            metaCell.AddElement(new Paragraph("INVOICE", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, AccentColor))
            {
                Alignment = Element.ALIGN_RIGHT
            });
            metaCell.AddElement(new Paragraph($"#{bookingId:D6}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, TextDark))
            {
                Alignment     = Element.ALIGN_RIGHT,
                SpacingBefore = 2
            });
            metaCell.AddElement(new Paragraph($"Generated: {DateTime.Now:dd MMM yyyy}", FontMuted)
            {
                Alignment     = Element.ALIGN_RIGHT,
                SpacingBefore = 2
            });
            headerTable.AddCell(metaCell);

            doc.Add(headerTable);
            doc.Add(CreateDivider());
        }

        private static void AddBookingInfoSection(Document doc, Shared.HomeCare.Entities.Booking booking)
        {
            doc.Add(CreateSectionLabel("BOOKING DETAILS"));

            var table = new PdfPTable(4) { WidthPercentage = 100, SpacingBefore = 4 };
            table.SetWidths(new float[] { 25, 25, 25, 25 });

            string statusLabel = booking.Status switch
            {
                BookingStatus.Confirmed => "Confirmed",
                BookingStatus.Completed => "Completed",
                BookingStatus.Cancelled => "Cancelled",
                _                       => "Pending"
            };
            BaseColor statusBg = booking.Status switch
            {
                BookingStatus.Confirmed => AccentColor,
                BookingStatus.Completed => GreenColor,
                BookingStatus.Cancelled => new BaseColor(220, 38, 38),
                _                       => new BaseColor(161, 161, 170)
            };

            AddInfoCard(table, "Booking ID", $"#{booking.Id:D6}");
            AddInfoCard(table, "Date",       booking.BookingDate.ToString("dd MMM yyyy"));
            AddInfoCard(table, "Time",       booking.BookingTime);
            AddStatusCard(table, "Status",   statusLabel, statusBg);

            doc.Add(table);
        }

        private static void AddServiceSection(Document doc, Shared.HomeCare.Entities.Booking booking)
        {
            doc.Add(CreateSectionLabel("SERVICE INFORMATION"));

            var table = new PdfPTable(3) { WidthPercentage = 100, SpacingBefore = 4 };
            table.SetWidths(new float[] { 40, 35, 25 });

            AddInfoCard(table, "Service",  booking.Service?.Name            ?? "-");
            AddInfoCard(table, "Category", booking.ServiceType?.ServiceName ?? "-");
            AddInfoCard(table, "Duration", $"{booking.DurationInMinutes} Min");

            doc.Add(table);
        }

        private static void AddPartnerSection(Document doc, string partnerName)
        {
            doc.Add(CreateSectionLabel("ASSIGNED SERVICE PARTNER"));

            var table = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 4 };

            var cell = new PdfPCell
            {
                BackgroundColor = LightGray,
                Border          = Rectangle.BOX,
                BorderColor     = BorderGray,
                BorderWidth     = 0.5f,
                Padding         = 10
            };
            cell.AddElement(new Paragraph(partnerName, FontBold));
            cell.AddElement(new Paragraph(
                "This partner is assigned to your booking. Contact HomeCare if you have any concerns.",
                FontMuted)
            { SpacingBefore = 3 });

            table.AddCell(cell);
            doc.Add(table);
        }

        private static void AddAddressSection(Document doc, Shared.HomeCare.Entities.Booking booking)
        {
            doc.Add(CreateSectionLabel("SERVICE ADDRESS"));

            var address = booking.Address;
            if (address is null)
            {
                doc.Add(new Paragraph("No address available.", FontMuted));
                return;
            }

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(address.HouseFlatNumber)) parts.Add(address.HouseFlatNumber);
            if (!string.IsNullOrWhiteSpace(address.Landmark))        parts.Add(address.Landmark);
            if (!string.IsNullOrWhiteSpace(address.FullAddress))     parts.Add(address.FullAddress);

            var fullAddressLine = string.Join(", ", parts);

            var table = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 4 };
            var cell  = new PdfPCell
            {
                BackgroundColor = LightGray,
                Border          = Rectangle.BOX,
                BorderColor     = BorderGray,
                BorderWidth     = 0.5f,
                Padding         = 10
            };
            cell.AddElement(new Paragraph(fullAddressLine.Length > 0 ? fullAddressLine : "-", FontBody));
            if (!string.IsNullOrWhiteSpace(address.SaveAs))
                cell.AddElement(new Paragraph($"Saved as: {address.SaveAs}", FontMuted) { SpacingBefore = 3 });

            table.AddCell(cell);
            doc.Add(table);
        }

       private static void AddPaymentSummary(Document doc, Shared.HomeCare.Entities.Booking booking)
{
    doc.Add(CreateSectionLabel("PAYMENT SUMMARY"));

    var summaryTable = new PdfPTable(2)
    {
        WidthPercentage     = 100,
        HorizontalAlignment = Element.ALIGN_LEFT,
        SpacingBefore       = 4
    };
    summaryTable.SetWidths(new float[] { 60, 40 });

    string paymentMethod = booking.PaymentMethod switch
    {
        PaymentMethod.Card => "Card",
        PaymentMethod.Cash => "Cash",
        _                  => "N/A"
    };

    string paymentStatus = booking.PaymentStatus switch
    {
        PaymentStatus.Success => "Paid",
        PaymentStatus.Pending => "Pending",
        PaymentStatus.Failed  => "Failed",
        _                     => "-"
    };

    var discountPct    = booking.Offer?.DiscountPercentage ?? 0;
    var discountAmount  = discountPct > 0
        ? Math.Round(booking.BookingAmount / (1 - discountPct / 100) * (discountPct / 100), 2)
        : 0;
    var originalAmount = booking.BookingAmount + discountAmount;
    var couponCode     = booking.Offer?.CouponCode;

    var discountLabel  = couponCode is not null
        ? $"Discount ({couponCode} - {discountPct}%)"
        : "Discount";

    AddSummaryRow(summaryTable, "Service Amount", $"${originalAmount:F2}",        isBold: false);
    AddSummaryRow(summaryTable, discountLabel,    $"-${discountAmount:F2}",        isBold: false);
    AddSummaryRowDivider(summaryTable);
    AddSummaryRow(summaryTable, "Total Amount",   $"${booking.BookingAmount:F2}",  isBold: true, isTotal: true);
    AddSummaryRow(summaryTable, "Payment Method", paymentMethod,                   isBold: false);
    AddSummaryRow(summaryTable, "Payment Status", paymentStatus,
        isBold: true,
        statusColor: booking.PaymentStatus == PaymentStatus.Success ? GreenColor : null);

    doc.Add(summaryTable);
}
        private static void AddFooter(Document doc)
        {
            doc.Add(CreateDivider());

            var footerTable = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 4 };
            footerTable.SetWidths(new float[] { 60, 40 });
            footerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            var leftCell = new PdfPCell { Border = Rectangle.NO_BORDER };
            leftCell.AddElement(new Paragraph(
                "Thank you for choosing HomeCare!",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, PrimaryColor)));
            leftCell.AddElement(new Paragraph(
                "For support, contact support@homecare.com", FontMuted)
            { SpacingBefore = 3 });
            footerTable.AddCell(leftCell);

            var rightCell = new PdfPCell
            {
                Border              = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };
            rightCell.AddElement(new Paragraph(
                $"Generated on {DateTime.Now:dd MMM yyyy, hh:mm tt}", FontMuted)
            { Alignment = Element.ALIGN_RIGHT });
            footerTable.AddCell(rightCell);

            doc.Add(footerTable);
        }

        private static Paragraph CreateSectionLabel(string text)
        {
            return new Paragraph(text, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, TextMuted))
            {
                SpacingBefore = 8,
                SpacingAfter  = 2
            };
        }

        private static PdfPTable CreateDivider()
        {
            var line = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 4, SpacingAfter = 4 };
            line.AddCell(new PdfPCell
            {
                BackgroundColor = BorderGray,
                FixedHeight     = 1f,
                Border          = Rectangle.NO_BORDER
            });
            return line;
        }

        private static void AddInfoCard(PdfPTable table, string label, string value)
        {
            var cell = new PdfPCell
            {
                BackgroundColor = LightGray,
                Border          = Rectangle.BOX,
                BorderColor     = BorderGray,
                BorderWidth     = 0.5f,
                Padding         = 8
            };
            cell.AddElement(new Paragraph(label, FontMuted));
            cell.AddElement(new Paragraph(value, FontBold) { SpacingBefore = 3 });
            table.AddCell(cell);
        }

        private static void AddStatusCard(PdfPTable table, string label, string value, BaseColor bgColor)
        {
            var cell = new PdfPCell
            {
                BackgroundColor = LightGray,
                Border          = Rectangle.BOX,
                BorderColor     = BorderGray,
                BorderWidth     = 0.5f,
                Padding         = 8
            };
            cell.AddElement(new Paragraph(label, FontMuted));

            var badgeTable = new PdfPTable(1) { WidthPercentage = 60, SpacingBefore = 4 };
            badgeTable.AddCell(new PdfPCell(new Phrase(value, FontStatus))
            {
                BackgroundColor     = bgColor,
                Border              = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding             = 4
            });
            cell.AddElement(badgeTable);
            table.AddCell(cell);
        }

        private static void AddSummaryRow(
            PdfPTable table,
            string label,
            string value,
            bool isBold,
            bool isTotal = false,
            BaseColor? statusColor = null)
        {
            var labelFont = isBold ? FontBold : FontBody;
            var valueFont = statusColor is not null
                ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, statusColor)
                : (isBold
                    ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, isTotal ? PrimaryColor : TextDark)
                    : FontBody);

            var bg = isTotal ? new BaseColor(238, 240, 255) : BaseColor.WHITE;

            table.AddCell(new PdfPCell(new Phrase(label, labelFont))
            {
                BackgroundColor = bg,
                Border          = Rectangle.BOX,
                BorderColor     = BorderGray,
                BorderWidth     = 0.5f,
                Padding         = 6   
            });

            table.AddCell(new PdfPCell(new Phrase(value, valueFont))
            {
                BackgroundColor     = bg,
                Border              = Rectangle.BOX,
                BorderColor         = BorderGray,
                BorderWidth         = 0.5f,
                Padding             = 6,  
                HorizontalAlignment = Element.ALIGN_RIGHT
            });
        }

        private static void AddSummaryRowDivider(PdfPTable table)
        {
            table.AddCell(new PdfPCell
            {
                Colspan         = 2,
                BackgroundColor = PrimaryColor,
                FixedHeight     = 1.5f,
                Border          = Rectangle.NO_BORDER
            });
        }
    }
}