﻿namespace Desktop_Nhom13.Models.Assignments
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int AssignmentID { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
