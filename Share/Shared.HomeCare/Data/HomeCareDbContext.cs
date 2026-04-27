using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;

namespace Infrastructure.HomeCare.Data
{
    public class HomeCareDbContext : DbContext
    {
        public HomeCareDbContext(DbContextOptions<HomeCareDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Otp> Otps { get; set; } = null!;
        public DbSet<UserAddress> UserAddresses { get; set; } = null!;

        public DbSet<AdminUser> Admins { get; set; } = null!;
        public DbSet<AdminPasswordResetToken> AdminPasswordResetTokens { get; set; } = null!;
        public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; } = null!;

        public DbSet<ServiceTypes> ServiceTypes { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<SubCategory> SubCategories { get; set; } = null!;
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<Offer> Offers { get; set; } = null!;

        public DbSet<ServicesOfSubCategory> Services { get; set; } = null!;
        public DbSet<ServicesImages> ServicesImages { get; set; } = null!;
        public DbSet<ServiceInclusionExclusion> ServiceInclusionsExclusions { get; set; } = null!;

        public DbSet<ServicePartner> ServicePartners { get; set; } = null!;
        public DbSet<ServicePartnerAttachment> ServicePartnerAttachments { get; set; } = null!;
        public DbSet<ServicePartnerEducation> ServicePartnerEducations { get; set; } = null!;
        public DbSet<ServicePartnerExperience> ServicePartnerExperiences { get; set; } = null!;
        public DbSet<ServicePartnerLanguage> ServicePartnerLanguages { get; set; } = null!;
        public DbSet<ServicePartnerServiceOffered> ServicePartnerServicesOffered { get; set; } = null!;
        public DbSet<ServicePartnerSkill> ServicePartnerSkills { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<CouponUsage> CouponUsages { get; set; } = null!;
        public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType, b =>
                {
                    b.Property("CreatedBy").HasColumnName("created_by");
                    b.Property("CreatedAt").HasColumnName("created_at");
                    b.Property("ModifiedBy").HasColumnName("modified_by");
                    b.Property("ModifiedAt").HasColumnName("modified_at");
                    b.Property("IsDeleted").HasColumnName("is_deleted");
                });
            }

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("user_name").HasMaxLength(150);
                b.Property(x => x.Email).HasColumnName("user_email").HasMaxLength(255).IsRequired();
                b.Property(x => x.IsEmailVerified).HasColumnName("is_email_verified");
                b.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasMaxLength(15);
                b.Property(x => x.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Otp>(b =>
            {
                b.ToTable("user_otps");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Email).HasColumnName("otp_email").IsRequired();
                b.Property(x => x.Code).HasColumnName("otp_code").HasMaxLength(64).IsRequired();
                b.Property(x => x.ExpiryAt).HasColumnName("otp_expiry_at");
                b.Property(x => x.IsUsed).HasColumnName("otp_is_used");
                b.Property(x => x.CreatedAt).HasColumnName("otp_created_at");
                b.Property(x => x.RefreshTokenHash).HasColumnName("refresh_token_hash").HasMaxLength(64);
                b.Property(x => x.RefreshTokenExpiryAt).HasColumnName("refresh_token_expiry_at");
            });

            modelBuilder.Entity<UserAddress>(b =>
            {
                b.ToTable("user_addresses");
                b.HasKey(x => x.AddressId);
                b.Property(x => x.AddressId).HasColumnName("id");
                b.Property(x => x.UserId).HasColumnName("user_id");
                b.Property(x => x.HouseFlatNumber).HasColumnName("house_flat_number").HasMaxLength(100).IsRequired();
                b.Property(x => x.Landmark).HasColumnName("landmark").HasMaxLength(200);
                b.Property(x => x.FullAddress).HasColumnName("full_address").HasMaxLength(500);
                b.Property(x => x.SaveAs).HasColumnName("save_as").HasMaxLength(50);
                b.Property(x => x.Latitude).HasColumnName("latitude").HasColumnType("numeric(10,7)");
                b.Property(x => x.Longitude).HasColumnName("longitude").HasColumnType("numeric(10,7)");
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => x.UserId);
            });

            modelBuilder.Entity<AdminUser>(b =>
            {
                b.ToTable("admins");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
                b.Property(x => x.MobileNumber).HasColumnName("mobile_number").HasMaxLength(15).IsRequired();
                b.Property(x => x.Address).HasColumnName("address").HasMaxLength(255);
                b.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                b.Property(x => x.ProfileImageName).HasColumnName("profile_image_name").HasMaxLength(255);
                b.Property(x => x.IsSuperAdmin).HasColumnName("is_super_admin");
                b.HasIndex(x => x.Email).IsUnique();
                b.HasMany(x => x.PasswordResetTokens).WithOne(t => t.Admin)
                    .HasForeignKey(t => t.AdminId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.RefreshTokens).WithOne(t => t.Admin)
                    .HasForeignKey(t => t.AdminId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AdminPasswordResetToken>(b =>
            {
                b.ToTable("admin_password_reset_tokens");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.AdminId).HasColumnName("admin_id").IsRequired();
                b.Property(x => x.Token).HasColumnName("token").HasMaxLength(255).IsRequired();
                b.Property(x => x.ExpiresAt).HasColumnName("expires_at");
                b.Property(x => x.IsUsed).HasColumnName("is_used");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.HasIndex(x => x.Token).IsUnique();
            });

            modelBuilder.Entity<AdminRefreshToken>(b =>
            {
                b.ToTable("admin_refresh_tokens");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.AdminId).HasColumnName("admin_id").IsRequired();
                b.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(512).IsRequired();
                b.Property(x => x.ExpiresAt).HasColumnName("expires_at");
                b.Property(x => x.IsRevoked).HasColumnName("is_revoked");
                b.Property(x => x.CreatedAt).HasColumnName("created_at");
                b.Property(x => x.ReplacedByTokenHash).HasColumnName("replaced_by_token_hash").HasMaxLength(512);
                b.HasIndex(x => x.TokenHash).IsUnique();
            });

            modelBuilder.Entity<ServiceTypes>(b =>
            {
                b.ToTable("service_types");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServiceName).HasColumnName("service_name").HasMaxLength(150).IsRequired();
                b.Property(x => x.ImageName).HasColumnName("image_path").HasMaxLength(300);
            });

            modelBuilder.Entity<Category>(b =>
            {
                b.ToTable("categories");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(150).IsRequired();
                b.Property(x => x.IsActive).HasColumnName("is_active");
                b.Property(x => x.ServiceTypeId).HasColumnName("service_id");
                b.HasOne(x => x.ServiceTypes).WithMany(s => s.Categories).HasForeignKey(x => x.ServiceTypeId);
            });

            modelBuilder.Entity<SubCategory>(b =>
            {
                b.ToTable("sub_categories");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.SubCategoryName).HasColumnName("sub_category_name").HasMaxLength(150).IsRequired();
                b.Property(x => x.IsActive).HasColumnName("is_active");
                b.Property(x => x.CategoryId).HasColumnName("category_id");
                b.HasOne(x => x.Category).WithMany(c => c.SubCategories).HasForeignKey(x => x.CategoryId);
            });

            modelBuilder.Entity<Language>(b =>
            {
                b.ToTable("languages");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("language_name").IsRequired();
            });

            modelBuilder.Entity<Offer>(b =>
            {
                b.ToTable("offers");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.CouponCode).HasColumnName("coupon_code").HasMaxLength(100).IsRequired();
                b.Property(x => x.CouponDescription).HasColumnName("coupon_description").HasMaxLength(250);
                b.Property(x => x.DiscountPercentage).HasColumnName("discount_percentage").HasColumnType("decimal(5,2)");
                b.Property(x => x.AppliedCount).HasColumnName("applied_count");
                b.Property(x => x.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<ServicesOfSubCategory>(b =>
            {
                b.ToTable("services");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("service_name").HasMaxLength(150).IsRequired();
                b.Property(x => x.SubCategoryId).HasColumnName("sub_category_id");
                b.Property(x => x.Duration).HasColumnName("duration_minutes").HasMaxLength(50).IsRequired();
                b.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
                b.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(10,2)").IsRequired();
                b.Property(x => x.Commission).HasColumnName("commission").HasColumnType("decimal(5,2)").IsRequired();
                b.Property(x => x.IsAvailable).HasColumnName("is_available").IsRequired();
                b.HasOne(x => x.SubCategory).WithMany(sc => sc.Services).HasForeignKey(x => x.SubCategoryId);
            });

            modelBuilder.Entity<ServicesImages>(b =>
            {
                b.ToTable("services_images");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServiceId).HasColumnName("service_id").IsRequired();
                b.Property(x => x.ImageName).HasColumnName("image_name").HasMaxLength(300).IsRequired();
                b.HasOne(x => x.Service).WithMany(s => s.Images).HasForeignKey(x => x.ServiceId);
            });

            modelBuilder.Entity<ServiceInclusionExclusion>(b =>
            {
                b.ToTable("service_inclusions_exclusions");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServiceId).HasColumnName("service_id").IsRequired();
                b.Property(x => x.Item).HasColumnName("item").HasMaxLength(300).IsRequired();
                b.Property(x => x.Type).HasColumnName("type").IsRequired();
                b.HasOne(x => x.Service).WithMany(s => s.ServiceFilters).HasForeignKey(x => x.ServiceId);
            });

            modelBuilder.Entity<ServicePartner>(b =>
            {
                b.ToTable("service_partners");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.FullName).HasColumnName("full_name").IsRequired();
                b.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
                b.Property(x => x.Gender).HasColumnName("gender").HasConversion<string>();
                b.Property(x => x.MobileNumber).HasColumnName("mobile_number").IsRequired();
                b.Property(x => x.Email).HasColumnName("email").IsRequired();
                b.Property(x => x.ApplyingForTypeId).HasColumnName("applying_for_type_id");
                b.Property(x => x.PermanentAddress).HasColumnName("permanent_address").IsRequired();
                b.Property(x => x.ResidentialAddress).HasColumnName("residential_address").IsRequired();
                b.Property(x => x.ProfileImageUrl).HasColumnName("profile_image_url");
                b.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
                b.Property(x => x.VerificationStatus).HasColumnName("verification_status").HasConversion<string>();
                b.Property(x => x.VerifiedAt).HasColumnName("verified_at");
                b.Property(x => x.VerifiedBy).HasColumnName("verified_by");
                b.Property(x => x.RejectionReason).HasColumnName("rejection_reason");
                b.Property(x => x.TotalJobsCompleted).HasColumnName("total_jobs_completed");
                b.HasOne(x => x.ServiceTypes)
                 .WithMany()
                 .HasForeignKey(x => x.ApplyingForTypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ServicePartnerAttachment>(b =>
            {
                b.ToTable("service_partner_attachments");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.FileUrl).HasColumnName("file_url").IsRequired();
                b.Property(x => x.FileName).HasColumnName("file_name").IsRequired();
                b.Property(x => x.FileType).HasColumnName("file_type");
                b.Property(x => x.FileSizeKb).HasColumnName("file_size_kb");
                b.Property(x => x.DocumentLabel).HasColumnName("document_label");
            });

            modelBuilder.Entity<ServicePartnerEducation>(b =>
            {
                b.ToTable("service_partner_educations");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.SchoolCollege).HasColumnName("school_college").IsRequired();
                b.Property(x => x.PassingYear).HasColumnName("passing_year");
                b.Property(x => x.Marks).HasColumnName("marks");
            });

            modelBuilder.Entity<ServicePartnerExperience>(b =>
            {
                b.ToTable("service_partner_experiences");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.CompanyName).HasColumnName("company_name").IsRequired();
                b.Property(x => x.Role).HasColumnName("role").IsRequired();
                b.Property(x => x.FromDate).HasColumnName("from_date");
                b.Property(x => x.ToDate).HasColumnName("to_date");
            });

            modelBuilder.Entity<ServicePartnerLanguage>(b =>
            {
                b.ToTable("service_partner_languages");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.LanguageId).HasColumnName("language_id");
                b.Property(x => x.Proficiency).HasColumnName("proficiency").HasConversion<string>();
            });

            modelBuilder.Entity<ServicePartnerServiceOffered>(b =>
            {
                b.ToTable("service_partner_services_offered");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.SubCategoryId).HasColumnName("sub_category_id");
            });

            modelBuilder.Entity<ServicePartnerSkill>(b =>
            {
                b.ToTable("service_partner_skills");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ServicePartnerId).HasColumnName("sp_id");
                b.Property(x => x.CategoryId).HasColumnName("category_id");
            });

            modelBuilder.Entity<Booking>(b =>
            {
                b.ToTable("bookings");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                b.Property(x => x.ServiceId).HasColumnName("service_id").IsRequired();
                b.Property(x => x.ServiceTypeId).HasColumnName("service_type_id").IsRequired();
                b.Property(x => x.AssignedPartnerId).HasColumnName("assigned_partner_id");
                b.Property(x => x.AddressId).HasColumnName("address_id").IsRequired();
                b.Property(x => x.OfferId).HasColumnName("offer_id");
                b.Property(x => x.DurationInMinutes).HasColumnName("duration").IsRequired();
                b.Property(x => x.BookingDate).HasColumnName("booking_date").IsRequired();
                b.Property(x => x.BookingTime).HasColumnName("booking_time").HasMaxLength(5).IsRequired();
                b.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasConversion<string>();
                b.Property(x => x.BookingAmount).HasColumnName("booking_amount").HasColumnType("decimal(10,2)");
                b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(BookingStatus.Pending);
                b.Property(x => x.PaymentStatus).HasColumnName("payment_status").HasConversion<string>();
                b.Property(x => x.CancellationReason).HasColumnName("cancellation_reason").HasMaxLength(500);
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.AssignedPartner).WithMany().HasForeignKey(x => x.AssignedPartnerId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
                b.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.Offer).WithMany().HasForeignKey(x => x.OfferId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
                b.HasIndex(x => x.UserId); b.HasIndex(x => x.Status); b.HasIndex(x => x.ServiceTypeId);
                b.HasIndex(x => new { x.BookingDate, x.BookingTime });
            });

            modelBuilder.Entity<Transaction>(b =>
            {
                b.ToTable("transactions");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.BookingId).HasColumnName("booking_id").IsRequired();
                b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                b.Property(x => x.ServiceId).HasColumnName("service_id").IsRequired();
                b.Property(x => x.TransactionAmount).HasColumnName("transaction_amount").HasColumnType("decimal(10,2)");
                b.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasConversion<string>();
                b.Property(x => x.TransactionDate).HasColumnName("transaction_date");
                b.Property(x => x.TransactionId).HasColumnName("transaction_uuid").IsRequired();
                b.Property(x => x.PaymentStatus).HasColumnName("payment_status").HasConversion<string>();
                b.Property(x => x.StripePaymentIntentId).HasColumnName("stripe_payment_intent_id").HasMaxLength(255);
                b.HasOne(x => x.Booking).WithOne(bk => bk.Transaction).HasForeignKey<Transaction>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(x => x.TransactionId).IsUnique();
                b.HasIndex(x => x.BookingId).IsUnique();
            });

            modelBuilder.Entity<SupportTicket>(b =>
            {
                b.ToTable("support_tickets");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
                b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
                b.Property(x => x.ContactNumber).HasColumnName("contact_number").HasMaxLength(15).IsRequired();
                b.Property(x => x.Description).HasColumnName("description").IsRequired();
                b.Property(x => x.SubmittedAt).HasColumnName("submitted_at");
            });

            modelBuilder.Entity<CouponUsage>(b =>
            {
                b.ToTable("coupon_usages");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                b.Property(x => x.CouponId).HasColumnName("coupon_id").IsRequired();
                b.Property(x => x.BookingId).HasColumnName("booking_id").IsRequired();
                b.Property(x => x.UsedAt).HasColumnName("used_at");
                b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.Coupon).WithMany().HasForeignKey(x => x.CouponId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(x => x.Booking).WithOne(bk => bk.CouponUsage).HasForeignKey<CouponUsage>(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(x => new { x.UserId, x.CouponId, x.BookingId }).IsUnique();
            });
        }
    }
}