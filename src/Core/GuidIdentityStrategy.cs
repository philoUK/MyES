namespace Core
{
    using System;

    public class GuidIdentityStrategy : IdentityStrategy
    {
        private Guid id;

        public GuidIdentityStrategy()
        {
            this.id = Guid.NewGuid();
        }

        public string GetId()
        {
            return this.id.ToString();
        }

        public void SetId(string newId)
        {
            this.id = new Guid(newId);
        }
    }
}