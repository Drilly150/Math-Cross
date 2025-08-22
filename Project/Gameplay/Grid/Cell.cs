namespace MathCross.Gameplay
{
    /// <summary>
    /// Representa una única celda en el tablero de juego Math-Cross.
    /// Esta clase funciona como una estructura de datos simple.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// El valor mostrado en la celda, como un número ("5"), un operador ("+") o un igual ("=").
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Determina si el jugador puede editar el contenido de esta celda.
        /// Será 'false' para las pistas dadas y 'true' para los espacios vacíos a rellenar.
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Constructor para crear una nueva celda.
        /// </summary>
        /// <param name="value">El valor inicial de la celda.</param>
        /// <param name="isEditable">Si la celda es editable por el jugador.</param>
        public Cell(string value, bool isEditable)
        {
            Value = value;
            IsEditable = isEditable;
        }
    }
}