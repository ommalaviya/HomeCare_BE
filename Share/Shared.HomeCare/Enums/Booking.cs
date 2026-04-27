namespace Shared.HomeCare.Enums
{
    public enum PaymentMethod
    {
        Card = 1,
        Cash = 2
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum PaymentStatus
    {
        Failed = 1,
        Success = 2,
        Pending = 3
    }
    public enum BookingTab
    {
        Upcoming = 1,
        Completed = 2
    }
}