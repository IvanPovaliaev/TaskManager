using System.Collections.Generic;
using TaskManager.Common.Models;
using ClosedXML.Excel;

namespace TaskManager.Client.Services
{
    public class ExcelService
    {
        public List<UserModel> GetAllUsers(string filePath)
        {
            var userModels = new List<UserModel>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var firstSheet = workbook.Worksheet(1);
                var itemIndex = 1;

                while (true)
                {
                    var name = firstSheet.Cell(itemIndex, "A").GetText();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var surname = firstSheet.Cell(itemIndex, "B").GetText();
                        var email = firstSheet.Cell(itemIndex, "C").GetText();
                        var phone = firstSheet.Cell(itemIndex, "D").GetText();
                        var password = firstSheet.Cell(itemIndex, "E").GetText();

                        var role = UserRole.User;
                        var userModel = new UserModel(name, surname, email, password, role, phone);
                        userModels.Add(userModel);
                        itemIndex++;
                        continue;
                    }
                    break;
                }
            }

            return userModels;
        }
    }
}
