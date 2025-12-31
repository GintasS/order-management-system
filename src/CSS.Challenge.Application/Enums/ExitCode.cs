namespace CSS.Challenge.Application.Enums
{
    /// <summary>
    /// Possible exit codes returned by the console application.
    /// </summary>
    public enum  ExitCode
    {
        Success,    
        ErrorOnGetOrdersEmptyResponse,
        ErrorProcessingOrders,
        ErrorPostOrders
    }
}
