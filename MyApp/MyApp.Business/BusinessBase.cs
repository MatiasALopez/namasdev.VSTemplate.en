using AutoMapper;
using namasdev.Core.Validation;

namespace MyApp.Business
{
    public class BusinessBase<TRespositorio>
        where TRespositorio : class
    {
        private readonly TRespositorio _repository;
        private readonly IErrorsBusiness _errorsBusiness;
        private readonly IMapper _mapper;

        public BusinessBase(TRespositorio repository, IErrorsBusiness errorsBusiness, IMapper mapper)
        {
            Validator.ValidateRequiredArgumentAndThrow(repository, nameof(repository));
            Validator.ValidateRequiredArgumentAndThrow(errorsBusiness, nameof(errorsBusiness));
            Validator.ValidateRequiredArgumentAndThrow(mapper, nameof(mapper));

            _repository = repository;
            _errorsBusiness = errorsBusiness;
            _mapper = mapper;
        }

        protected TRespositorio Repository
        {
            get { return _repository; }
        }

        protected IErrorsBusiness ErrorsBusiness
        {
            get { return _errorsBusiness; }
        }

        protected IMapper Mapper
        {
            get { return _mapper; }
        }
    }
}
