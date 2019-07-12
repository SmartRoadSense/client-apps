using System;
namespace SmartRoadSense
{
    public enum OutcomeEvents { success, error };

    public class Outcome<T, E> where E : Exception
    {
        OutcomeEvents _event;
        T _element;
        E _error;

        public Outcome(T element)
        {
            _event = OutcomeEvents.success;
            _element = element;
        }

        public Outcome(E error)
        {
            _event = OutcomeEvents.error;
            _error = error;
        }

        public OutcomeEvents Event
        {
            get { return _event; }
        }

        public T Element
        {
            get { return _element; }
        }

        public E Error
        {
            get { return _error; }
        }
    }

    public class Outcome<E> where E : Exception
    {
        OutcomeEvents _event;
        E _error;

        public Outcome()
        {
            _event = OutcomeEvents.success;
        }

        public Outcome(E error)
        {
            _event = OutcomeEvents.error;
            _error = error;
        }

        public OutcomeEvents Event
        {
            get { return _event; }
        }

        public E Error
        {
            get { return _error; }
        }
    }
}
