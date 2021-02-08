namespace AcApi.Infrastructure.Exception
{
    public class InvalidaDataExpcetion : BusinessRuleExpcetion
    {
        public InvalidaDataExpcetion() : base() { }
        public InvalidaDataExpcetion(string message) : base(message) { }
    }
}
