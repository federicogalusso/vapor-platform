using Exceptions;

namespace Entities
{
    public class Rate
    {
        private int _score;
        private string _comment;
        public required string GameName { get; set; }
        public int Score
        {
            get => _score;
            set
            {
                if (IsScoreValid(value))
                {
                    _score = value;
                }
                else
                {
                    throw new DomainException("La puntuacion debe ser entre 1 y 5");
                }
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new DomainException("El comentario no puede estar vacio");
                }
                _comment = value;
            }
        }

        private bool IsScoreValid(int score)
        {
            return score >= 1 && score <= 5;
        }

        public override string ToString()
        {
            return $"Puntaje: {Score}, Comentario: {Comment}";
        }
    }
}
