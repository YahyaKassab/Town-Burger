namespace Town_Burger.Models.Responses
{
    public class GenericResponse<T>
    {
        public bool IsSuccess{ get; set; }
        public string Message{ get; set; }
        public string[]? Errors{ get; set; }
        public T? Result{ get; set; }
        
    }
}
