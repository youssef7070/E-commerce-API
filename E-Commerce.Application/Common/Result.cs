using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class Result
    {

        public bool IsSuccess { get; }

        public IReadOnlyList<Error> Errors { get; }

        protected Result( bool isSucces , IReadOnlyList<Error> erros )
        {

            IsSuccess = isSucces;

            Errors = erros;

        }

        public static Result Ok => new Result(true , Array.Empty<Error>()); 

        public static Result Fail(Error error) => new Result(false , new[] { error });

        public static Result Fail(IReadOnlyList<Error> erros) => new Result(false , erros);



    }


    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public TValue data => IsSuccess ? _value : throw new InvalidOperationException("can not access the value of failed result");


        private Result(TValue value) : base(true, Array.Empty<Error>())
        {

            _value = value;

        }


        private Result(Error error) : base(false, new[] { error })
        {

            _value = default!;

        }

        private Result(IReadOnlyList<Error> errors):base(false, errors)
        {

            _value = default!;

        }

        public static Result<TValue> Ok(TValue value) => new (value);

        public static Result<TValue> Fail(Error error) => new (error);

        public static Result<TValue> Fail(IReadOnlyList<Error> erros) => new (erros);

        public static implicit operator Result<TValue>(TValue value) => Ok(value);

        public static implicit operator Result<TValue>(Error error) => Fail(error);


    }



}
