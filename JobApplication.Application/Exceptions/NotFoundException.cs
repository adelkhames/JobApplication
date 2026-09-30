namespace JobApplication.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resource, object key)
            : base($"{resource} with id '{key}' was not found.") { }
    }
}
