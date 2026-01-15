using System.Globalization;
using Exceptions;

namespace Entities
{
    public class Game
    {
        private string _title;
        private string _type;
        private string _launchDate;
        private string _platform;
        private string _publisher;
        private int _availableUnits;
        private string _owner;

        public required string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("El titulo no puede estar vacio.");
                }
                _title = value;
            }

        }
        public required string Type
        {
            get => _type;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("El tipo no puede estar vacio.");
                }
                _type = value;
            }
        }
        public required string LaunchDate
        {
            get => _launchDate ?? "";
            set
            {
                if (!IsDateFormatValid(value))
                {
                    throw new DomainException("El formato de la fecha debe ser MM/DD/YYYY.");
                }

                if (IsDateInFuture(value))
                {
                    throw new DomainException("La fecha de lanzamiento no puede ser futura.");
                }

                _launchDate = value;
            }
        }
        public required string Platform
        {
            get => _platform;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("La plataforma no puede estar vacio.");
                }
                _platform = value;
            }
        }
        public required string Publisher
        {
            get => _publisher;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("El publicador no puede estar vacio.");
                }
                _publisher = value;
            }
        }
        public required int AvailableUnits
        {
            get => _availableUnits;
            set
            {
                if (!IsValidNumber(value))
                {
                    throw new DomainException("Error: La cantidad de unidades debe ser un numero valido.");
                }
                _availableUnits = value;
            }
        }
        
        public required string Owner
        {
            get => _owner;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("El dueño del juego no puede estar vacio.");
                }
                _owner = value;
            }
        }
        public string? Image { get; set; }

        public override string ToString()
        {
            return $"Titulo: {Title}, Tipo: {Type}, Fecha de Lanzamiento: {LaunchDate}, Plataforma: {Platform}, Publicador: {Publisher}, Unidades disponibles: {AvailableUnits}";
        }

        private bool IsDateFormatValid(string date)
        {
            if (DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDateInFuture(string date)
        {
            DateTime parsedDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            return parsedDate > DateTime.Now;
        }

        private bool IsValidNumber(int value)
        {
            return value >= 0;
        }
    }

}