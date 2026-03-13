using Microsoft.AspNetCore.Components;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public partial class About
    {
        [Inject] private EnrollmentLogic EnrollmentLogic { get; set; } = default!;

        public IQueryable<EnrollmentStat> GetEnrollmentData(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
        {
            var data = EnrollmentLogic.GetEnrollmentsByDate();
            totalRowCount = data.Count;
            return data.AsQueryable();
        }
    }
}
