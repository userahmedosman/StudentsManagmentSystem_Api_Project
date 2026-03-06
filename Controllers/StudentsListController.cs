
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentBusinessLayer;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/Students")]
    [Authorize]

    public class StudentsController : ControllerBase // Declare the controller class inheriting from ControllerBase.
    {


        [HttpGet("All", Name = "GetAllStudents")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //here we used StudentDTO
        public ActionResult<IEnumerable<StudentReturnDTO>> GetAllStudents() // Define a method to get all students.
        {
            //if (StudentDataSimulation.StudentsList.Count == 0) 
            //{
            //    return NotFound("No Students Found!");
            //}
            //return Ok(StudentDataSimulation.StudentsList); // Returns the list of students.

            List<StudentReturnDTO> StudentsList = Student.GetAllStudents();
            if (StudentsList.Count == 0)
            {
                return NotFound("No Students Found!");
            }
            return Ok(StudentsList); // Returns the list of students.

        }

        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        // Method to get all students who passed
        public ActionResult<IEnumerable<StudentReturnDTO>> GetPassedStudents()

        {
            // var passedStudents = StudentDataSimulation.StudentsList.Where(student => student.GPA >= 50).ToList();

            List<StudentReturnDTO> PassedStudentsList = Student.GetPassedStudents();
            if (PassedStudentsList.Count == 0)
            {
                return NotFound("No Students Found!");
            }

            return Ok(PassedStudentsList); // Return the list of students who passed.
        }

        [HttpGet("AverageGPA", Name = "GetAverageGPA")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<decimal> GetAverageGPA()
        {
            //var averageGPA = StudentDataSimulation.StudentsList.Average(student => student.GPA);
            decimal averageGPA = Student.GetAverageGPA();
            return Ok(averageGPA);
        }


        [HttpGet("{id}", Name = "GetStudentById")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public ActionResult<StudentReturnDTO> GetStudentById(int id)
        {

            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            Student student = Student.Find(id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            //here we get only the DTO object to send it back.
            StudentReturnDTO StudentDTO = student.SRDTO;

            //we return the DTO not the student object.
            return Ok(StudentDTO);

        }

        //for add new we use Http Post

        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudentDTO)
        {
            //we validate the data here
            if (newStudentDTO == null || string.IsNullOrEmpty(newStudentDTO.Name) || newStudentDTO.Age < 0 || newStudentDTO.GPA < 0)
            {
                return BadRequest("Invalid student data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            Student student = new Student(new StudentDTO(newStudentDTO.Id, newStudentDTO.Name, newStudentDTO.Age, newStudentDTO.GPA, newStudentDTO.Email, newStudentDTO.PasswordHash, newStudentDTO.Role));
            student.Save();

            newStudentDTO.Id = student.ID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.Id }, newStudentDTO);

        }



        //here we use http put method for update
        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudent)
        {
            if (id < 1 || updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age < 0 || updatedStudent.GPA < 0)
            {
                return BadRequest("Invalid student data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            Student student = Student.FindStudnetToUpdate(id);


            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }


            student.Name = string.IsNullOrEmpty(updatedStudent.Name) ? student.Name : updatedStudent.Name;
            student.Age = updatedStudent.Age <= 0 ? student.Age : updatedStudent.Age;
            student.GPA = updatedStudent.GPA <= 0 ? student.GPA : updatedStudent.GPA;
            student.Email = string.IsNullOrEmpty(updatedStudent.Email) ? student.Email : updatedStudent.Email;
            student.PasswordHash = string.IsNullOrEmpty(updatedStudent.PasswordHash) ? student.PasswordHash : updatedStudent.PasswordHash;
            student.Role = string.IsNullOrEmpty(updatedStudent.Role) ? student.Role : updatedStudent.Role;
            student.Save();

            //we return the DTO not the full student object.
            return Ok(student.SDTO);

        }


        //here we use HttpDelete method
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (Student.DeleteStudent(id))

                return Ok($"Student with ID {id} has been deleted.");
            else
                return NotFound($"Student with ID {id} not found. no rows deleted!");
        }


        [HttpPost("Upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0) return BadRequest("No file uploaded");

            var uploadLocation = @"C:\UploadImageDir";
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadLocation, fileName);

            if (!Directory.Exists(uploadLocation)) Directory.CreateDirectory(uploadLocation);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { filePath });
        }

        [HttpGet("GetImage/{fileName}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetImage(string fileName)
        {
            var imageDir = @"C:\UploadImageDir";
            var filePath = Path.Combine(imageDir, fileName);

            if (!System.IO.File.Exists(filePath)) return NotFound("Image not found");
            var image = System.IO.File.OpenRead(filePath);
            var mimeType = GetMimeType(filePath);

            return File(image, mimeType);
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".jpeg" or ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

    }
}
