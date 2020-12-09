using System;
using System.Collections.Generic;
using LenovoController.Providers;

namespace LenovoController.Features
{
    public class AbstractWmiFeature<T> : IFeature<T> where T : struct, IComparable
    {
        private readonly string _methodNameSuffix;
        private readonly int _offset;

        protected AbstractWmiFeature(string methodNameSuffix, int offset)
        {
            _methodNameSuffix = methodNameSuffix;
            _offset = offset;
        }

        public T GetState()
        {
            return FromInternal(WmiProvider.ExecuteGamezone("Get" + _methodNameSuffix, "Data"));
        }

        public void SetState(T state)
        {
            WmiProvider.ExecuteGamezone("Set" + _methodNameSuffix, "Data",
                new Dictionary<string, string>
                {
                    {"Data", ToInternal(state).ToString()}
                });
        }

        private int ToInternal(T state)
        {
            return (int) (object) state + _offset;
        }

        private T FromInternal(int state)
        {
            return (T) (object) (state - _offset);
        }
    }
}