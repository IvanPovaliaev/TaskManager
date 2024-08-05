using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TaskManager.Common.Models.Services
{
    public class ValidationService
    {
        private const string _emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string _userNamePattern = @"^[A-Z][a-zA-Z-' ]*$";
        private const string _phoneNumbPattern = @"^\+?\d{1,3}[ -\(]?\d{3,4}[ -\)]?\d{3}[ -]?\d{2}[ -]?\d{2}";
        private const string _restrictedCharsPattern = @"[!@#$%^&*()+={}\[\]|\\:;""'<>,/?`~]";

        public ValidationService() { }

        public bool IsCorrectUserInputData(UserModel userModel, out List<string> messages)
        {
            var isCorrectInputData = true;
            messages = [];

            if (!IsCorrectUserName(userModel.FirstName))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect First name");
            }

            if (!IsCorrectUserName(userModel.Surname))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect Surname");
            }

            if (!IsCorrectEmail(userModel.Email))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect email");
            }
            if (userModel.Phone != null && !IsCorrectPhone(userModel.Phone))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect phone number");
            }

            return isCorrectInputData;
        }
        public bool IsCorrectProjectInputData(ProjectModel projectModel, out List<string> messages)
        {
            var isCorrectInputData = true;
            var regex = new Regex(_restrictedCharsPattern);
            messages = [];

            if (!string.IsNullOrEmpty(projectModel.Name) && regex.IsMatch(projectModel.Name))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect Name");
                messages.Add($"The project Name must not contain the following characters:\n{string.Join(" ", GetRestrictedChars())}");
            }

            return isCorrectInputData;
        }
        public bool IsCorrectDeskInputData(DeskModel deskModel, out List<string> messages)
        {
            var isCorrectInputData = true;
            var regex = new Regex(_restrictedCharsPattern);
            messages = [];

            if (!string.IsNullOrEmpty(deskModel.Name) && regex.IsMatch(deskModel.Name))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect Name");
                messages.Add($"The desk Name must not contain the following characters:\n{string.Join(" ", GetRestrictedChars())}");
            }

            return isCorrectInputData;
        }
        public bool IsCorrectTaskInputData(TaskModel taskModel, out List<string> messages)
        {
            var isCorrectInputData = true;
            var regex = new Regex(_restrictedCharsPattern);
            messages = [];

            if (!string.IsNullOrEmpty(taskModel.Name) && regex.IsMatch(taskModel.Name))
            {
                isCorrectInputData = false;
                messages.Add("Incorrect Name");
                messages.Add($"The task Name must not contain the following characters:\n{string.Join(" ", GetRestrictedChars())}");
            }

            return isCorrectInputData;
        }
        public bool IsCorrectEmail(string email)
        {
            if (email == null) return false;

            var regex = new Regex(_emailPattern);
            return regex.IsMatch(email);
        }
        public bool IsCorrectUserName(string? name)
        {
            if (name == null) return false;

            var regex = new Regex(_userNamePattern);
            return regex.IsMatch(name);
        }
        public bool IsCorrectPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return true;

            var regex = new Regex(_phoneNumbPattern);
            return regex.IsMatch(phone);
        }
        public char[] GetRestrictedChars()
        {
            var restrinctedRegex = new Regex(_restrictedCharsPattern);
            return restrinctedRegex.ToString().ToCharArray();
        }
    }
}
