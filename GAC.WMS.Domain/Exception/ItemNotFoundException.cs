namespace GAC.WMS.Domain.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(int id) : base($"Item with id {id} not found.")
        {
        
        }

        public ItemNotFoundException(string name) : base($"Item with name {name} not found.")
        {

        }
    }
}
