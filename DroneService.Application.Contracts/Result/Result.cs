namespace DroneService.Application.Contracts.Result;

// Základní třída pro reprezentaci výsledku operace (bez návratové hodnoty)
public class Result
{
    public bool Success { get; set; } // určuje, zda operace proběhla úspěšně
    public string? Error { get; set; } // chybová zpráva (pokud Success = false)

    // vytvoření úspěšného výsledku
    public static Result Ok() =>
        new Result
        {
            Success = true
        };

    // vytvoření neúspěšného výsledku s chybovou zprávou
    public static Result Fail(string error) =>
        new Result
        {
            Success = false,
            Error = error
        };
}