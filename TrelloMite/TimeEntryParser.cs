using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrelloMite
{
    public class TimeEntryParser
    {
        private const string DatePattern = @"(\d{1,4}[\.\-/]\d{1,2}[\.\-/]\d{1,4})";
        private const string DurationPattern = @"([\d\.,:hm]*)";

        public DateTime ParseDate(string text)
        {
            DateTime date;
            if (TryParseDate(text, out date))
            {
                return date;
            }

            throw new CannotParseException();
        }

        public bool TryParseDate(string text, out DateTime date)
        {
            date = new DateTime();
            var command = FindCommand(text, "time");
            var match = Regex.Match(command, @"^" + DatePattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return DateTime.TryParse(match.Groups[1].Value, new CultureInfo("de-AT"), DateTimeStyles.None, out date)
                       || DateTime.TryParse(match.Groups[1].Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            }

            return false;
        }

        public TimeSpan ParseDuration(string text)
        {
            TimeSpan duration;
            if (TryParseDuration(text, out duration))
            {
                return duration;
            }

            throw new CannotParseException();
        }

        public bool TryParseDuration(string text, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;
            var command = FindCommand(text, "time");
            var match = Regex.Match(command, DurationPattern + "$", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int minutes;
                if (TryParseMinutes(match.Groups[1].Value, out minutes))
                {
                    duration = TimeSpan.FromMinutes(minutes);
                    return true;
                }
            }

            return false;
        }

        public string ParseProject(string text)
        {
            string project;
            if (TryParseProject(text, out project))
            {
                return project;
            }

            throw new CannotParseException();
        }

        public bool TryParseProject(string text, out string project)
        {
            return TryFindCommand(text, "project", out project);
        }

        public string ParseCustomer(string text)
        {
            string customer;
            if (TryParseCustomer(text, out customer))
            {
                return customer;
            }

            throw new CannotParseException();
        }

        public bool TryParseCustomer(string text, out string customer)
        {
            return TryFindCommand(text, "customer", out customer);
        }

        public string ParseService(string text)
        {
            string service;
            if (TryParseService(text, out service))
            {
                return service;
            }

            throw new CannotParseException();
        }

        public bool TryParseService(string text, out string service)
        {
            return TryFindCommand(text, "service", out service);
        }

        public string ParseNotes(string text)
        {
            string notes;
            if (TryParseNotes(text, out notes))
            {
                return notes;
            }

            throw new CannotParseException();
        }

        public bool TryParseNotes(string text, out string notes)
        {
            return TryFindCommand(text, "notes", out notes);
        }

        private bool TryParseMinutes(string command, out int minutes)
        {
            minutes = -1;
            if (string.IsNullOrWhiteSpace(command))
                return false;

            command = command.Trim();

            double hoursInDouble;
            if (double.TryParse(command, NumberStyles.Any, new CultureInfo("de-AT"), out hoursInDouble))
                minutes = (int)(hoursInDouble * 60);
            if (double.TryParse(command, NumberStyles.Any, CultureInfo.InvariantCulture, out hoursInDouble))
                minutes = (int)(hoursInDouble * 60);

            if (command.ToLower().EndsWith("h"))
            {
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, new CultureInfo("de-AT"), out hoursInDouble))
                    minutes = (int)(hoursInDouble * 60);
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, CultureInfo.InvariantCulture, out hoursInDouble))
                    minutes = (int)(hoursInDouble * 60);
            }

            if (command.ToLower().EndsWith("m"))
            {
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, new CultureInfo("de-AT"), out hoursInDouble))
                    minutes = (int)(hoursInDouble);
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, CultureInfo.InvariantCulture, out hoursInDouble))
                    minutes = (int)(hoursInDouble);
            }

            if (command.Contains(":"))
            {
                int hours;

                int.TryParse(command.Substring(0, command.IndexOf(":", StringComparison.Ordinal)), out hours);
                int.TryParse(command.Substring(command.IndexOf(":", StringComparison.Ordinal) + 1), out minutes);

                minutes = hours * 60 + minutes;
            }

            if (minutes != -1)
                return true;

            return false;
        }

        private string FindCommand(string text, string command)
        {
            string parsedCommand;
            if (TryFindCommand(text, command, out parsedCommand))
            {
                return parsedCommand;
            }

            throw new CouldNotParseCommandException(command);
        }

        private bool TryFindCommand(string text, string command, out string parsedCommand)
        {
            parsedCommand = null;
            var match = Regex.Match(text, @"^@" + command + " (.{1,})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                parsedCommand = match.Groups[1].Value;
                return true;
            }

            return false;
        }
    }
}