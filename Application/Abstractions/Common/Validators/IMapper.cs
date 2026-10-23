namespace Application.Abstractions.Common.Validators;

public interface IMapper<E, D> where E : class
{
    D Map(E entity);
    List<D> Map(List<E> entities);
}