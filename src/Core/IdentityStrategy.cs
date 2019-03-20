namespace Core
{
    public interface IdentityStrategy
    {
        string GetId();

        void SetId(string newId);
    }
}
