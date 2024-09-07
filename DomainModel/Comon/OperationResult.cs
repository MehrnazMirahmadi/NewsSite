namespace DomainModel.Comon
{
    public class OperationResult
    {
        public string Message { get; private set; }
        public bool Success { get; private set; }
        public OperationResult ToSuccess(string Message)
        {
            this.Success = true;
            this.Message = Message;
            return this;

        }
        public OperationResult ToFailed(string Message)
        {
            this.Success = false;
            this.Message = Message;
            return this;

        }
    }
}
