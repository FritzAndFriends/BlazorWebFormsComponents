namespace AfterDepartmentPortal.Models;

/// <summary>
/// Static sample data provider — identical to DepartmentPortal.Models.PortalDataProvider
/// for apples-to-apples migration comparison.
/// </summary>
public static class PortalDataProvider
{
    public static List<Department> GetDepartments()
    {
        return new List<Department>
        {
            new Department { Id = 1, Name = "Engineering", DivisionName = "Technology", ManagerId = 1 },
            new Department { Id = 2, Name = "Human Resources", DivisionName = "Operations", ManagerId = 5 },
            new Department { Id = 3, Name = "Marketing", DivisionName = "Commercial", ManagerId = 9 },
            new Department { Id = 4, Name = "Finance", DivisionName = "Operations", ManagerId = 13 },
            new Department { Id = 5, Name = "Customer Support", DivisionName = "Commercial", ManagerId = 17 }
        };
    }

    public static List<Employee> GetEmployees()
    {
        return new List<Employee>
        {
            // Engineering
            new Employee { Id = 1, Name = "Alice Chen", Title = "VP of Engineering", Department = "Engineering", Email = "achen@contoso.com", Phone = "(555) 100-0001", PhotoUrl = "/images/employees/alice.png", HireDate = new DateTime(2015, 3, 15), IsAdmin = true },
            new Employee { Id = 2, Name = "Bob Martinez", Title = "Senior Developer", Department = "Engineering", Email = "bmartinez@contoso.com", Phone = "(555) 100-0002", PhotoUrl = "/images/employees/bob.png", HireDate = new DateTime(2017, 7, 1), IsAdmin = false },
            new Employee { Id = 3, Name = "Carol Washington", Title = "Software Engineer", Department = "Engineering", Email = "cwashington@contoso.com", Phone = "(555) 100-0003", PhotoUrl = "/images/employees/carol.png", HireDate = new DateTime(2019, 1, 10), IsAdmin = false },
            new Employee { Id = 4, Name = "David Kim", Title = "DevOps Engineer", Department = "Engineering", Email = "dkim@contoso.com", Phone = "(555) 100-0004", PhotoUrl = "/images/employees/david.png", HireDate = new DateTime(2020, 6, 22), IsAdmin = false },

            // Human Resources
            new Employee { Id = 5, Name = "Elena Ruiz", Title = "HR Director", Department = "Human Resources", Email = "eruiz@contoso.com", Phone = "(555) 200-0001", PhotoUrl = "/images/employees/elena.png", HireDate = new DateTime(2014, 9, 5), IsAdmin = true },
            new Employee { Id = 6, Name = "Frank Okafor", Title = "Recruiter", Department = "Human Resources", Email = "fokafor@contoso.com", Phone = "(555) 200-0002", PhotoUrl = "/images/employees/frank.png", HireDate = new DateTime(2018, 11, 18), IsAdmin = false },
            new Employee { Id = 7, Name = "Grace Liu", Title = "Benefits Coordinator", Department = "Human Resources", Email = "gliu@contoso.com", Phone = "(555) 200-0003", PhotoUrl = "/images/employees/grace.png", HireDate = new DateTime(2021, 2, 14), IsAdmin = false },
            new Employee { Id = 8, Name = "Hector Patel", Title = "Training Specialist", Department = "Human Resources", Email = "hpatel@contoso.com", Phone = "(555) 200-0004", PhotoUrl = "/images/employees/hector.png", HireDate = new DateTime(2019, 8, 30), IsAdmin = false },

            // Marketing
            new Employee { Id = 9, Name = "Irene Novak", Title = "Marketing Director", Department = "Marketing", Email = "inovak@contoso.com", Phone = "(555) 300-0001", PhotoUrl = "/images/employees/irene.png", HireDate = new DateTime(2016, 4, 12), IsAdmin = true },
            new Employee { Id = 10, Name = "James Thompson", Title = "Content Strategist", Department = "Marketing", Email = "jthompson@contoso.com", Phone = "(555) 300-0002", PhotoUrl = "/images/employees/james.png", HireDate = new DateTime(2020, 1, 7), IsAdmin = false },
            new Employee { Id = 11, Name = "Karen Yamamoto", Title = "Graphic Designer", Department = "Marketing", Email = "kyamamoto@contoso.com", Phone = "(555) 300-0003", PhotoUrl = "/images/employees/karen.png", HireDate = new DateTime(2021, 5, 20), IsAdmin = false },
            new Employee { Id = 12, Name = "Leo Santos", Title = "SEO Analyst", Department = "Marketing", Email = "lsantos@contoso.com", Phone = "(555) 300-0004", PhotoUrl = "/images/employees/leo.png", HireDate = new DateTime(2022, 3, 1), IsAdmin = false },

            // Finance
            new Employee { Id = 13, Name = "Maria Johansson", Title = "CFO", Department = "Finance", Email = "mjohansson@contoso.com", Phone = "(555) 400-0001", PhotoUrl = "/images/employees/maria.png", HireDate = new DateTime(2013, 6, 1), IsAdmin = true },
            new Employee { Id = 14, Name = "Nathan Brooks", Title = "Senior Accountant", Department = "Finance", Email = "nbrooks@contoso.com", Phone = "(555) 400-0002", PhotoUrl = "/images/employees/nathan.png", HireDate = new DateTime(2018, 10, 15), IsAdmin = false },
            new Employee { Id = 15, Name = "Olivia Grant", Title = "Financial Analyst", Department = "Finance", Email = "ogrant@contoso.com", Phone = "(555) 400-0003", PhotoUrl = "/images/employees/olivia.png", HireDate = new DateTime(2020, 7, 22), IsAdmin = false },
            new Employee { Id = 16, Name = "Paul Nguyen", Title = "Payroll Specialist", Department = "Finance", Email = "pnguyen@contoso.com", Phone = "(555) 400-0004", PhotoUrl = "/images/employees/paul.png", HireDate = new DateTime(2021, 9, 10), IsAdmin = false },

            // Customer Support
            new Employee { Id = 17, Name = "Quinn Harper", Title = "Support Manager", Department = "Customer Support", Email = "qharper@contoso.com", Phone = "(555) 500-0001", PhotoUrl = "/images/employees/quinn.png", HireDate = new DateTime(2016, 12, 3), IsAdmin = false },
            new Employee { Id = 18, Name = "Rachel Adams", Title = "Support Specialist", Department = "Customer Support", Email = "radams@contoso.com", Phone = "(555) 500-0002", PhotoUrl = "/images/employees/rachel.png", HireDate = new DateTime(2019, 4, 18), IsAdmin = false },
            new Employee { Id = 19, Name = "Samuel Lee", Title = "Technical Support", Department = "Customer Support", Email = "slee@contoso.com", Phone = "(555) 500-0003", PhotoUrl = "/images/employees/samuel.png", HireDate = new DateTime(2020, 11, 5), IsAdmin = false },
            new Employee { Id = 20, Name = "Tina Volkov", Title = "Support Analyst", Department = "Customer Support", Email = "tvolkov@contoso.com", Phone = "(555) 500-0004", PhotoUrl = "/images/employees/tina.png", HireDate = new DateTime(2022, 1, 25), IsAdmin = false }
        };
    }

    public static List<Announcement> GetAnnouncements()
    {
        return new List<Announcement>
        {
            new Announcement { Id = 1, Title = "Welcome to the New Department Portal", Body = "We're excited to launch our new internal portal. Explore employee directories, training courses, and company resources all in one place.", Author = "Elena Ruiz", PublishDate = new DateTime(2025, 1, 2), IsActive = true },
            new Announcement { Id = 2, Title = "Q1 All-Hands Meeting Scheduled", Body = "Join us on January 15th for the quarterly all-hands meeting. We'll cover company goals, department updates, and recognize outstanding contributions.", Author = "Alice Chen", PublishDate = new DateTime(2025, 1, 5), IsActive = true },
            new Announcement { Id = 3, Title = "Updated Remote Work Policy", Body = "Effective February 1st, the company is adopting a hybrid work model. Employees may work remotely up to three days per week with manager approval.", Author = "Elena Ruiz", PublishDate = new DateTime(2025, 1, 10), IsActive = true },
            new Announcement { Id = 4, Title = "IT Security Training Mandatory", Body = "All employees must complete the annual IT Security Awareness training by January 31st. Access the course through the Training section of the portal.", Author = "David Kim", PublishDate = new DateTime(2025, 1, 12), IsActive = true },
            new Announcement { Id = 5, Title = "New Health Benefits Enrollment", Body = "Open enrollment for 2025 health benefits begins February 1st. Review plan options and make selections through the HR portal by February 28th.", Author = "Grace Liu", PublishDate = new DateTime(2025, 1, 15), IsActive = true },
            new Announcement { Id = 6, Title = "Employee Appreciation Week", Body = "Mark your calendars for Employee Appreciation Week, March 3-7. Activities include team lunches, recognition awards, and a company-wide celebration.", Author = "Hector Patel", PublishDate = new DateTime(2025, 1, 18), IsActive = true },
            new Announcement { Id = 7, Title = "Office Renovation Phase 2", Body = "The second phase of office renovations will begin on February 10th, affecting floors 3 and 4. Temporary workspaces will be provided.", Author = "Maria Johansson", PublishDate = new DateTime(2025, 1, 20), IsActive = true },
            new Announcement { Id = 8, Title = "Annual Performance Reviews", Body = "Annual performance review forms are now available. Managers should schedule review meetings with direct reports before March 15th.", Author = "Elena Ruiz", PublishDate = new DateTime(2025, 1, 22), IsActive = true },
            new Announcement { Id = 9, Title = "New Parking Garage Access", Body = "The new employee parking garage is now open. Employees can request parking passes through the Facilities section of the portal.", Author = "Quinn Harper", PublishDate = new DateTime(2025, 1, 25), IsActive = false },
            new Announcement { Id = 10, Title = "Summer Internship Program Applications", Body = "The 2025 Summer Internship Program is now accepting applications. Department managers can submit intern requests through the HR portal.", Author = "Frank Okafor", PublishDate = new DateTime(2025, 1, 28), IsActive = true }
        };
    }

    public static List<TrainingCourse> GetCourses()
    {
        return new List<TrainingCourse>
        {
            new TrainingCourse { Id = 1, CourseName = "IT Security Awareness", Description = "Annual mandatory training covering phishing prevention, password security, and data protection best practices.", Instructor = "David Kim", DurationHours = 2, Category = "Compliance" },
            new TrainingCourse { Id = 2, CourseName = "Leadership Fundamentals", Description = "Develop essential leadership skills including communication, delegation, and team motivation.", Instructor = "Elena Ruiz", DurationHours = 8, Category = "Management" },
            new TrainingCourse { Id = 3, CourseName = "Agile Project Management", Description = "Learn Scrum and Kanban methodologies for effective software project delivery.", Instructor = "Alice Chen", DurationHours = 16, Category = "Technical" },
            new TrainingCourse { Id = 4, CourseName = "Effective Communication", Description = "Improve written and verbal communication skills for professional success.", Instructor = "Hector Patel", DurationHours = 4, Category = "Professional Development" },
            new TrainingCourse { Id = 5, CourseName = "Cloud Architecture Basics", Description = "Introduction to cloud computing concepts, AWS and Azure fundamentals.", Instructor = "Bob Martinez", DurationHours = 12, Category = "Technical" },
            new TrainingCourse { Id = 6, CourseName = "Diversity and Inclusion", Description = "Understanding and promoting diversity, equity, and inclusion in the workplace.", Instructor = "Grace Liu", DurationHours = 3, Category = "Compliance" },
            new TrainingCourse { Id = 7, CourseName = "Financial Planning for Employees", Description = "Learn about retirement planning, investment basics, and company benefits.", Instructor = "Nathan Brooks", DurationHours = 2, Category = "Professional Development" },
            new TrainingCourse { Id = 8, CourseName = "Customer Service Excellence", Description = "Techniques for delivering outstanding customer experiences and handling difficult situations.", Instructor = "Quinn Harper", DurationHours = 6, Category = "Professional Development" },
            new TrainingCourse { Id = 9, CourseName = "Data Analytics with Excel", Description = "Advanced Excel techniques including pivot tables, VLOOKUP, and data visualization.", Instructor = "Olivia Grant", DurationHours = 8, Category = "Technical" },
            new TrainingCourse { Id = 10, CourseName = "Workplace Safety", Description = "Mandatory workplace safety training covering emergency procedures and ergonomics.", Instructor = "Hector Patel", DurationHours = 1, Category = "Compliance" },
            new TrainingCourse { Id = 11, CourseName = "Public Speaking Workshop", Description = "Build confidence and skill in presenting to groups of all sizes.", Instructor = "Irene Novak", DurationHours = 4, Category = "Professional Development" },
            new TrainingCourse { Id = 12, CourseName = "Introduction to Machine Learning", Description = "Explore the fundamentals of machine learning algorithms and their applications.", Instructor = "Carol Washington", DurationHours = 20, Category = "Technical" },
            new TrainingCourse { Id = 13, CourseName = "Time Management Strategies", Description = "Practical techniques for prioritizing tasks, managing deadlines, and improving productivity.", Instructor = "Maria Johansson", DurationHours = 3, Category = "Professional Development" },
            new TrainingCourse { Id = 14, CourseName = "Content Marketing Fundamentals", Description = "Learn to create compelling content that drives engagement and supports business goals.", Instructor = "James Thompson", DurationHours = 6, Category = "Marketing" },
            new TrainingCourse { Id = 15, CourseName = "Conflict Resolution", Description = "Strategies for managing and resolving workplace conflicts constructively.", Instructor = "Elena Ruiz", DurationHours = 4, Category = "Management" }
        };
    }

    public static List<Resource> GetResources()
    {
        return new List<Resource>
        {
            new Resource { Id = 1, Title = "Employee Handbook 2025", Description = "Complete guide to company policies, procedures, and employee benefits.", CategoryId = 1, CategoryName = "HR Policies", Url = "/resources/employee-handbook.pdf", FileType = "PDF" },
            new Resource { Id = 2, Title = "Remote Work Guidelines", Description = "Policies and best practices for working remotely.", CategoryId = 1, CategoryName = "HR Policies", Url = "/resources/remote-work-guide.pdf", FileType = "PDF" },
            new Resource { Id = 3, Title = "Code of Conduct", Description = "Company code of conduct and ethics guidelines.", CategoryId = 1, CategoryName = "HR Policies", Url = "/resources/code-of-conduct.pdf", FileType = "PDF" },
            new Resource { Id = 4, Title = "IT Setup Guide", Description = "Instructions for setting up your workstation, VPN, and development tools.", CategoryId = 2, CategoryName = "IT Resources", Url = "/resources/it-setup-guide.pdf", FileType = "PDF" },
            new Resource { Id = 5, Title = "VPN Configuration", Description = "Step-by-step VPN setup instructions for remote access.", CategoryId = 2, CategoryName = "IT Resources", Url = "/resources/vpn-config.pdf", FileType = "PDF" },
            new Resource { Id = 6, Title = "Software Request Form", Description = "Form to request new software installations or licenses.", CategoryId = 2, CategoryName = "IT Resources", Url = "/resources/software-request.docx", FileType = "DOCX" },
            new Resource { Id = 7, Title = "Expense Report Template", Description = "Standard template for submitting expense reimbursement requests.", CategoryId = 3, CategoryName = "Finance", Url = "/resources/expense-template.xlsx", FileType = "XLSX" },
            new Resource { Id = 8, Title = "Travel Policy", Description = "Guidelines for business travel, booking, and expense limits.", CategoryId = 3, CategoryName = "Finance", Url = "/resources/travel-policy.pdf", FileType = "PDF" },
            new Resource { Id = 9, Title = "Purchase Order Process", Description = "How to submit and track purchase orders.", CategoryId = 3, CategoryName = "Finance", Url = "/resources/po-process.pdf", FileType = "PDF" },
            new Resource { Id = 10, Title = "Brand Style Guide", Description = "Official brand guidelines including logos, colors, and typography.", CategoryId = 4, CategoryName = "Marketing", Url = "/resources/brand-guide.pdf", FileType = "PDF" },
            new Resource { Id = 11, Title = "Social Media Policy", Description = "Guidelines for representing the company on social media.", CategoryId = 4, CategoryName = "Marketing", Url = "/resources/social-media-policy.pdf", FileType = "PDF" },
            new Resource { Id = 12, Title = "Presentation Template", Description = "Official company PowerPoint template for presentations.", CategoryId = 4, CategoryName = "Marketing", Url = "/resources/presentation-template.pptx", FileType = "PPTX" },
            new Resource { Id = 13, Title = "Onboarding Checklist", Description = "New employee onboarding checklist for managers.", CategoryId = 5, CategoryName = "Training", Url = "/resources/onboarding-checklist.pdf", FileType = "PDF" },
            new Resource { Id = 14, Title = "Mentorship Program Guide", Description = "Information about the employee mentorship program.", CategoryId = 5, CategoryName = "Training", Url = "/resources/mentorship-guide.pdf", FileType = "PDF" },
            new Resource { Id = 15, Title = "Performance Review Form", Description = "Annual performance review form for managers and employees.", CategoryId = 5, CategoryName = "Training", Url = "/resources/review-form.docx", FileType = "DOCX" },
            new Resource { Id = 16, Title = "Emergency Procedures", Description = "Building emergency procedures and evacuation routes.", CategoryId = 6, CategoryName = "Facilities", Url = "/resources/emergency-procedures.pdf", FileType = "PDF" },
            new Resource { Id = 17, Title = "Conference Room Booking Guide", Description = "How to reserve conference rooms and AV equipment.", CategoryId = 6, CategoryName = "Facilities", Url = "/resources/room-booking.pdf", FileType = "PDF" },
            new Resource { Id = 18, Title = "Parking Pass Application", Description = "Form to request a parking pass for the employee garage.", CategoryId = 6, CategoryName = "Facilities", Url = "/resources/parking-pass.pdf", FileType = "PDF" },
            new Resource { Id = 19, Title = "Benefits Summary 2025", Description = "Overview of all employee benefits including health, dental, and vision.", CategoryId = 1, CategoryName = "HR Policies", Url = "/resources/benefits-summary.pdf", FileType = "PDF" },
            new Resource { Id = 20, Title = "Development Environment Standards", Description = "Coding standards, source control policies, and CI/CD pipeline documentation.", CategoryId = 2, CategoryName = "IT Resources", Url = "/resources/dev-standards.pdf", FileType = "PDF" }
        };
    }
}
