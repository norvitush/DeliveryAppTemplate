public interface IOrdersDataProvider : IModel
{
    int SelectedOrder { get; }

    void SetSelectedOrder(int id);
}
