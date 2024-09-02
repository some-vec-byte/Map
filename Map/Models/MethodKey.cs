using System;

namespace Map
{
    /// <summary>
    /// Eine Klasse welche einen Schlüssel für eine MapMethode in einem Dictionary darstellt.
    /// </summary>
    internal class MethodKey : Tuple<Type, Type>
    {

        /// <summary>
        /// Erstellt einen neue Instanz der Klasse.
        /// </summary>
        /// <param name="item1">Der Typ F</param>
        /// <param name="item2">Der Typ T</param>
        public MethodKey(Type item1, Type item2) : base(item1, item2)
        {
        }
    }
}