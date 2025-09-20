using Ardalis.Result;
using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.Services
{
    public abstract class BaseService
    {
        protected bool TryValidate<T>(T dto, out Result result)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var context = new ValidationContext(dto);
            var resultados = new List<ValidationResult>();

            if (Validator.TryValidateObject(dto, context, resultados, true))
            {
                result = Result.Success();
                return true;
            }
            else
            {
                result = Result.Invalid(resultados.Select(r => new ValidationError(r.ErrorMessage)));
                return false;
            }
        }
    }
}
