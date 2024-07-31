using System.Collections.Generic;
using TaskManager.Common.Models;
using ClosedXML.Excel;
using System.IO;

namespace TaskManager.Client.Services
{
    public class ExcelService
    {
        public List<UserModel> GetAllUsers(string filePath)
        {
            var fileName = filePath.Split('\\', System.StringSplitOptions.RemoveEmptyEntries)[^1];
            var tempFilePath = Path.GetTempPath() + fileName;
            File.Copy(filePath, tempFilePath, true);

            var userModels = new List<UserModel>();
            using (var workbook = new XLWorkbook(tempFilePath))
            {
                var firstSheet = workbook.Worksheet(1);
                var itemIndex = 1;

                while (true)
                {
                    var name = firstSheet.Cell(itemIndex, 1).Value.ToString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var surname = firstSheet.Cell(itemIndex, 2).Value.ToString();
                        var email = firstSheet.Cell(itemIndex, 3).Value.ToString();
                        var phone = firstSheet.Cell(itemIndex, 4).Value.ToString();
                        var password = firstSheet.Cell(itemIndex, 5).Value.ToString();

                        var role = UserRole.User;
                        var userModel = new UserModel(name, surname, email, password, role, phone);
                        userModels.Add(userModel);
                        itemIndex++;
                        continue;
                    }
                    break;
                }
            }

            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);

            return userModels;
        }
    }
}
