using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Collections;
using Dapper;

namespace nss.Data
{
    public class DatabaseInterface
    {
        public static SqliteConnection Connection
        {
            get
            {
                /*
                    Mac users: You can create an environment variable in your
                    .zshrc file.
                        export NSS_DB="/path/to/your/project/nss.db"

                    Windows users: You need to use a property window
                        http://www.forbeslindesay.co.uk/post/42833119552/permanently-set-environment-variables-on-windows
                 */
                string env = $"{Environment.GetEnvironmentVariable("NSS_DB")}";
                string _connectionString = $"Data Source={env}";
                return new SqliteConnection(_connectionString);
            }
        }


        public static void CheckCohortTable()
        {
            SqliteConnection db = DatabaseInterface.Connection;

            try
            {
                List<Cohort> cohorts = db.Query<Cohort>
                    ("SELECT Id FROM Cohort").ToList();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    db.Execute(@"CREATE TABLE Cohort (
                        `Id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        `Name`	TEXT NOT NULL UNIQUE
                    )");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Evening Cohort 1')");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Day Cohort 10')");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Day Cohort 11')");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Day Cohort 12')");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Day Cohort 13')");

                    db.Execute(@"INSERT INTO Cohort
                        VALUES (null, 'Day Cohort 21')");

                }
            }
        }

        public static void CheckInstructorsTable()
        {
            SqliteConnection db = DatabaseInterface.Connection;

            try
            {
                List<Instructor> toys = db.Query<Instructor>
                    ("SELECT Id FROM Instructor").ToList();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    db.Execute($@"CREATE TABLE Instructor (
                        `Id`	integer NOT NULL PRIMARY KEY AUTOINCREMENT,
                        `FirstName`	varchar(80) NOT NULL,
                        `LastName`	varchar(80) NOT NULL,
                        `SlackHandle`	varchar(80) NOT NULL,
                        `Specialty` varchar(80),
                        `CohortId`	integer NOT NULL,
                        FOREIGN KEY(`CohortId`) REFERENCES `Cohort`(`Id`)
                    )");

                    db.Execute($@"INSERT INTO Instructor
                        SELECT null,
                              'Steve',
                              'Brownlee',
                              '@coach',
                              'Dad jokes',
                              c.Id
                        FROM Cohort c WHERE c.Name = 'Evening Cohort 1'
                    ");

                    db.Execute($@"INSERT INTO Instructor
                        SELECT null,
                              'Joe',
                              'Shepherd',
                              '@joes',
                              'Analogies',
                              c.Id
                        FROM Cohort c WHERE c.Name = 'Day Cohort 13'
                    ");

                    db.Execute($@"INSERT INTO Instructor
                        SELECT null,
                              'Jisie',
                              'David',
                              '@jisie',
                              'Student success',
                              c.Id
                        FROM Cohort c WHERE c.Name = 'Day Cohort 21'");
                }
            }
        }

        public static void exercisesTable()
        {
            SqliteConnection db = DatabaseInterface.Connection;

            try
            {
                List<Exercise> exercises = db.Query<Exercise>
                    ("SELECT Id FROM Exercises").ToList();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    db.Execute($@"
					CREATE TABLE Exercises (
						`Id`	integer NOT NULL PRIMARY KEY AUTOINCREMENT,
						`Name`	varchar(80) NOT NULL,
						`Language`	varchar(80) NOT NULL
						)
					");

                    db.Execute($@"INSERT INTO Exercises
                        SELECT null,
                              'OverlyExcited',
                              'Javascript',
                    ");

                    db.Execute($@"INSERT INTO Exercises
                        SELECT null,
                              'ChickenMonkey',
                              'Javascript',
                    ");

                    db.Execute($@"INSERT INTO Exercises
                        SELECT null,
                              'Battle of the Bands',
                              'Javascript',
                    ");
                }
            }
        }
        public static void studentTable()
        {
            SqliteConnection db = DatabaseInterface.Connection;

            try
            {
                List<Student> exercises = db.Query<Student>
                    ("SELECT Id FROM Students").ToList();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    db.Execute($@"
					CREATE TABLE Students (
						`Id`	integer NOT NULL PRIMARY KEY AUTOINCREMENT,
						`FirstName`	varchar(80) NOT NULL,
						`LastName`	varchar(80) NOT NULL,
                        `SlackHandle` varchar(80) NOT NULL,
                        `Cohort` varchar(80) NOT NULL,
                        FOREIGN KEY(`CohortId`) REFERENCES `Cohort`(`Id`)
						)
					");

                    db.Execute($@"INSERT INTO Students
                        SELECT null,
                              'Emily',
                              'Venturino',
                              '@emilyv',
                              c.id FROM Cohort c WHERE c.Name = 'Day Cohort 26'
                              
                    ");

                    db.Execute($@"INSERT INTO Students
                        SELECT null,
                              'Elizabeth',
                              'Smith',
                              '@liz',
                              c.id FROM Cohort c WHERE c.Name = 'Day Cohort 13'
                    ");

                    db.Execute($@"INSERT INTO Students
                        SELECT null,
                              'Allison',
                              'Richardson',
                              '@allisonr',
                              c.id FROM Cohort c WHERE c.Name = 'Day Cohort 16'
                    ");
                }
            }
        }
        public static void studentExerciseTable()
        {
            SqliteConnection db = DatabaseInterface.Connection;

            try
            {
                List<StudentExercise> exercises = db.Query<StudentExercise>
                    ("SELECT Id FROM Students").ToList();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no such table"))
                {
                    db.Execute($@"

                    CREATE TABLE StudentExercise(
						`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
						`ExerciseId` INTEGER NOT NULL,
                        `StudentId` INTEGER NOT NULL,
                        `InstructorId` INTEGER NOT NULL,
                        FOREIGN KEY(`ExerciseId`) REFERENCES `Exercise`(`Id`),
                        FOREIGN KEY(`StudentId`) REFERENCES `Student`(`Id`),
                        FOREIGN KEY(`InstructorId`) REFERENCES `Instructor`(`Id`)
					)");

                    db.Execute($@"
					INSERT INTO StudentExercise
						SELECT null,
						e.Id,
                        s.Id, 
                        i.Id
                        FROM Student s, Exercise e, Instructor i
                        WHERE e.Name = 'Overly Excited'
                        AND s.SlackHandle = '@emilyv'
                        AND i.SlackHandle = '@coach'
            ");

                      db.Execute($@"
					INSERT INTO StudentExercise
						SELECT null,
						e.Id,
                        s.Id, 
                        i.Id
                        FROM Student s, Exercise e, Instructor i
                        WHERE e.Name = 'ChickenMonkey'
                        AND s.SlackHandle = '@liz'
                        AND i.SlackHandle = '@jisie'
            ");

                     db.Execute($@"
					INSERT INTO StudentExercise
						SELECT null,
						e.Id,
                        s.Id, 
                        i.Id
                        FROM Student s, Exercise e, Instructor i
                        WHERE e.Name = 'Battle Of the Bands'
                        AND s.SlackHandle = '@allisonr'
                        AND i.SlackHandle = '@joes'
            ");
                }
            }
        }
    }
}