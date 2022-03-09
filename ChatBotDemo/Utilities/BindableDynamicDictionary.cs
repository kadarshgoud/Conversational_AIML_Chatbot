using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace ChatBotDemo.Utilities
{
    public class BindableDynamicDictionary : DynamicObject, INotifyPropertyChanged, IEnumerable<object>
    {
        /// <summary>
        /// The internal dictionary.
        /// </summary>
        private readonly Dictionary<string, object> _dictionary;

        /// <summary>
        /// Creates a new BindableDynamicDictionary with an empty internal dictionary.
        /// </summary>
        public BindableDynamicDictionary()
        {
            _dictionary = new Dictionary<string, object>();
        }

        /// <summary>
        /// Copies the contents of the given dictionary to initilize the internal dictionary.
        /// </summary>
        /// <param name="source"></param>
        public BindableDynamicDictionary(IDictionary<string, object> source)
        {
            _dictionary = new Dictionary<string, object>(source);
        }
        /// <summary>
        /// You can still use this as a dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
                RaisePropertyChanged(key);
            }
        }

        /// <summary>
        /// This allows you to get properties dynamically.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// This allows you to set properties dynamically.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;
            RaisePropertyChanged(binder.Name);
            return true;
        }

        /// <summary>
        /// This is used to list the current dynamic members.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            //forces use of the non-generic implementation on the Values collection
            return ((IEnumerable)_dictionary.Values).GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            var propChange = PropertyChanged;
            if (propChange == null) return;
            propChange(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
