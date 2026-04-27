namespace Admin.Domain.HomeCare.DataModels.Request.Dashboard
{
    public class TotalServicesBookedRequestModel
    {
        public string Period { get; set; } = "year";
    }

    public class TopPerformingServicesRequestModel
    {
        public string Period { get; set; } = "week";
    }

    public class RevenueOverviewRequestModel
    {
        public string Period { get; set; } = "week";
    }
    public class CityBookingsChartRequestModel
    {
        public string Period { get; set; } = "week";
    }

    public class TopServicePartnersRequestModel
    {
        public int Top { get; set; } = 5;
    }
}