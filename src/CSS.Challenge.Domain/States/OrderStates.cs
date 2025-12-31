namespace CSS.Challenge.Domain.States
{
    /// <summary>
    /// Static class that holds class instances for all possible order states.
    /// The order states comes pre-defined with allowed transition states.
    /// </summary>
    public static class OrderStates
    {
        public static DiscardOrderState DiscardOrderStateInstance { get; } = new([]);

        public static PickOrderState PickOrderStateInstance { get; } = new([DiscardOrderStateInstance.GetType()]);

        public static MoveOrderState MoveOrderStateInstance { get; } = new([
            PickOrderStateInstance.GetType(),
            DiscardOrderStateInstance.GetType()
        ]);

        public static PlaceOrderState PlaceOrderStateInstance { get; } = new([
            MoveOrderStateInstance.GetType(),
            PickOrderStateInstance.GetType(),
            DiscardOrderStateInstance.GetType()
        ]);

        public static CookOrderState CookOrderStateInstance { get; } = new([
            PlaceOrderStateInstance.GetType(),
            PickOrderStateInstance.GetType(),
            DiscardOrderStateInstance.GetType()
        ]);

        public static PreprocessSingleOrderState PreprocessSingleOrderStateInstance { get; } = new([
            CookOrderStateInstance.GetType(),
            PickOrderStateInstance.GetType(),
            DiscardOrderStateInstance.GetType()
        ]);
    }
}
