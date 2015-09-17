using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter
{
    public sealed class Result<T, E>
    {
        public bool IsOk;
        public bool IsError => !IsOk;

        public T Value;
        public E Error;

        public Result(T value)
        {
            Value = value;
            IsOk = true;
        }

        public Result(E error)
        {
            Error = error;
            IsOk = false;
        }

        public static Result<Tx, Ex> Ok<Tx, Ex>(Tx value) => new Result<Tx, Ex>(value);
        public static Result<Tx, Ex> Err<Tx, Ex>(Ex error) => new Result<Tx, Ex>(error);

        public static implicit operator Result<T, E>(T value) => Ok<T, E>(value);
        public static implicit operator Result<T, E>(E error) => Err<T, E>(error);
    }
}
