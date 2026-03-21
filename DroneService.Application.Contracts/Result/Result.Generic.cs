namespace DroneService.Application.Contracts.Result;

// Generická verze Result třídy, která navíc obsahuje návratovou hodnotu
public class Result<T> : Result
{
    public T? Value { get; set; } // hodnota vrácená při úspěchu (např. DTO)

    // vytvoření úspěšného výsledku s hodnotou
    public static Result<T> Ok(T value) =>
        new Result<T>
        {
            Success = true,
            Value = value
        };

    // vytvoření neúspěšného výsledku s chybovou zprávou
    public static new Result<T> Fail(string error) =>
        new Result<T>
        {
            Success = false,
            Error = error
        };
}