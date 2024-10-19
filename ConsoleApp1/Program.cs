namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //First();
            //Second();
            //Third();
            //Fourth();
            Fifth();
        }

        public static void First()
        {
            string[] students = { "Ali", "Vali", "Lola", "Akmal" };
            int numberOfStudents = students.Length;

            int numberOfDays = 3;

            bool[,] attendance = new bool[numberOfStudents, numberOfDays];

            int day = 0;

            do
            {
                Console.WriteLine("Attendance for day " + (day + 1) + ":");
                for (int i = 0; i < numberOfStudents; i++)
                {
                    Console.WriteLine("Is " + students[i] + " present? (y/n)");
                    string input = Console.ReadLine().Trim().ToLower();

                    if (input == "y")
                        attendance[i, day] = true;
                    else if (input == "n")
                        attendance[i, day] = false;
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                        i--;
                    }
                }
                day++;
            }
            while (day < numberOfDays);
            Console.WriteLine("\nAttendance Summary:");
            for (int i = 0; i < numberOfStudents; i++)
            {
                int presentDay = 0;
                for (int j = 0; j < numberOfDays; j++)
                {
                    if (attendance[i, j])
                        presentDay++;
                }

                double attendancePercentage = (double)presentDay / numberOfDays * 100;
                Console.WriteLine("Student " + students[i] + " was present " + presentDay + " days out of " + numberOfDays + " days " + attendancePercentage + "%");
            }
            Console.ReadLine();
        }

        public static void Second()
        {
            string[] students = { "Ali", "Vali", "Lola", "Akmal" };
            int numberOfCandidates = students.Length;

            int[] votes = new int[numberOfCandidates];

            int numberOfStudents;
            Console.WriteLine("Enter the number of students voting:");
            if(!int.TryParse(Console.ReadLine(), out numberOfStudents) || numberOfStudents < 0)
            {
                Console.WriteLine("Please provide valid number of students");
                return;
            }

            int votesCast = 0;
            do
            {
                Console.WriteLine("\nVote for your class president");
                for (int i = 0; i < numberOfCandidates; i++)
                {
                    Console.WriteLine($"{i + 1}. {students[i]}");
                }
                Console.WriteLine("Enter the number of the candidate you want to vote for:");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int candidateNumber) && candidateNumber > 0 && candidateNumber <= numberOfCandidates)
                {
                    // Increment the vote count for the selected candidate
                    votes[candidateNumber - 1]++;
                    votesCast++;
                    Console.WriteLine("Vote recorded successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter a valid candidate number.");
                }
            }
            while (votesCast < numberOfStudents);

            Console.WriteLine("\nElection Results:");
            for (int i = 0; i < numberOfCandidates; i++)
            {
                Console.WriteLine($"{students[i]}: {votes[i]} votes");

            }

            int maxVotes = votes[0];
            int winnerIndex = 0;
            bool tie = false;

            for (int i = 0; i < numberOfCandidates; i++)
            {
                if (votes[i] > maxVotes)
                {
                    maxVotes = votes[i];
                    winnerIndex = i;
                    tie = false;
                }
                else if(votes[i] == maxVotes)
                {
                    tie = true;
                }
            }

            if(tie)
            {
                Console.WriteLine("It's a tie!");
            }
            else
            {
                Console.WriteLine($"The winner is {students[winnerIndex]} with {maxVotes} votes.");
            }

            Console.ReadLine();

        }

        public static void Third()
        {
            string[] courses = { "Math", "English", "Chemistry", "Biology", "History" };
            int numberOfCourses = courses.Length;

            int[] courseCapacity = { 30, 25, 20, 15, 10 };
            int[] seatsAvailable = (int[])courseCapacity.Clone();

            Dictionary<string, List<string>> enrolledStudents = new Dictionary<string, List<string>>();
            for (int i = 0; i < numberOfCourses; i++)
            {
                enrolledStudents[courses[i]] = new List<string>();
            }

            string input;
            do
            {
                Console.WriteLine("\nCourse Enrollment System");
                Console.WriteLine("Enter 'exit' to stop enrollment.");

                Console.WriteLine("Avaliable courses");
                for (int i = 0; i < numberOfCourses; i++)
                {
                    Console.WriteLine($"{i + 1}. {courses[i]} - {seatsAvailable[i]} seats available");
                }

                Console.WriteLine("Enter the student's name: ");
                string studentName = Console.ReadLine();

                if (studentName.Trim().ToLower() == "exit") break;

                Console.WriteLine("Enter the course number to enroll in: ");
                input = Console.ReadLine();

                if(int.TryParse(input, out int courseNumber) && courseNumber > 0 && courseNumber <= numberOfCourses)
                {
                    int courseIndex = courseNumber - 1;

                    if (seatsAvailable[courseIndex] > 0)
                    {
                        enrolledStudents[courses[courseIndex]].Add(studentName);
                        seatsAvailable[courseIndex]--;
                        Console.WriteLine($"{studentName} enrolled in {courses[courseIndex]}");
                    }
                    else
                    {
                        Console.WriteLine("The course is full. Please choose another course.");
                    }
                }
            } while (true);

            Console.WriteLine("\nFinal enrollment summary");
            for (int i = 0; i < numberOfCourses; i++)
            {
                Console.WriteLine($"\nCourses: {courses[i]}");
                if (enrolledStudents[courses[i]].Count > 0)
                {
                    Console.WriteLine("Enrolled students:");
                    foreach (var item in enrolledStudents[courses[i]])
                    {
                        Console.WriteLine($" - {item}");
                    }
                }
                else
                {
                    Console.WriteLine("No students enrolled in this course");
                }
                
            }


        }

        public static void Fourth()
        {
            string[] students = { "Ali", "Vali", "Lola", "Akmal" };
            int[] studentGrades = { 85, 92, 78, 91 };

            string input;
            do
            {
                Console.WriteLine("\nGradebook Search");
                Console.WriteLine("Enter 'exit' to stop searching");

                Console.WriteLine("Enter the student's name to search for: ");
                input = Console.ReadLine();

                if (input.Trim().ToLower() == "exit") break;
                bool found = false;
                for (int i = 0; i < students.Length; i++)
                {
                    if (students[i].Equals(input, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"{students[i]}'s grade: {studentGrades[i]}");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Student not found. Please try again");
                }
            }
            while (true);
        }

        public static void Fifth()
        {
            decimal[] accountBalances = { 1000.75m, 2000.12m, 3000.00m, 4000.50m, 5000.25m };
            int numberOfAccounts = accountBalances.Length;

            string input;
            do
            {
                Console.WriteLine("Bank Account System");
                Console.WriteLine("Enter 'exit' to stop transactions");

                Console.WriteLine("Account Balances");
                for (int i = 0; i < numberOfAccounts; i++)
                {
                    Console.WriteLine($"Account {i + 1}: {accountBalances[i]:C}");
                }

                Console.WriteLine("Enter the account number to transfer from:(1, 2, 3, 4 or 5)  ");
                input = Console.ReadLine();

                if (input.Trim().ToLower() == "exit") break;

                if (int.TryParse(input, out int accountNumber) && accountNumber > 0 && accountNumber <= numberOfAccounts)
                {
                    int accountIndex = accountNumber - 1;
                    Console.WriteLine("Enter 'd' for deposit 'w' for withdrawal: ");
                    string transactionType = Console.ReadLine();


                    Console.WriteLine("Enter the amount:");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                    {
                        if (transactionType == "d")
                        {
                            accountBalances[accountIndex] += amount;
                            Console.WriteLine("Deposit successful");
                        }
                        else if (transactionType == "w")
                        {
                            if (accountBalances[accountIndex] >= amount)
                            {
                                accountBalances[accountIndex] -= amount;
                                Console.WriteLine("Withdrawal successful");
                            }
                            else
                            {
                                Console.WriteLine("Insufficient funds");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid transaction type");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a positive number");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid account number. Please try again");
                }

            } while (true);
        }

    }
}
