namespace Core
{
    public class TextIdStrategy : IdentityStrategy
    {
        private string id;

        public string GetId()
        {
            return this.id;
        }

        public void SetId(string newId)
        {
            this.id = newId;
        }
    }
}
