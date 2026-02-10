const textarea = document.getElementById('stdId');
textarea.disabled = true;

//const cleanValue = textarea.value.replace(/[^\d\n]/g, '');

//textarea.value = cleanValue;


function disableButton() {
    if (textarea.disabled === false && textarea.value === "") {
        document.getElementById('fetchById').disabled = true;
    }
}
document.getElementById('fetchBtn').addEventListener('click', () => {
    if (textarea.disabled === false) {
        textarea.disabled = true;
    }
   FetchData("All");
});
document.getElementById('fetchPassedBtn').addEventListener('click', () => {
    if (textarea.disabled === false) {
        textarea.disabled = true;
    }
    textarea.disabled = true;
   FetchData("Passed");
});
document.getElementById('fetchById').addEventListener('click', () => {
    document.getElementById('studentsBody').innerHTML = '';
    textarea.disabled = false;
    if (textarea.value === "") return;
    disableButton();
    
   FetchData(textarea.value);
});


textarea.addEventListener('input', (e) => {
    // Replace anything that's not a digit or newline
    textarea.value = textarea.value.replace(/[^\d\n]/g, '');
    if (textarea.value !== "") {
        document.getElementById('fetchById').disabled = false;
    }
});

function FetchData(uri){
    fetch('https://localhost:7034/api/Students/'+uri) // Your actual API endpoint
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (Array.isArray(data)) {
                displayStudents(data);
            } else {
                displaySingleStudent(data)
            }
        })
        .catch(error => {
            alert('Error: ' + error.message);
        });
}

function displayStudents(students) {
    const tableBody = document.getElementById('studentsBody');
    tableBody.innerHTML = ''; // Clear previous data

    students.forEach(student => {
        const row = document.createElement('tr');

        row.innerHTML = `
            <td>${student.id}</td>
            <td>${student.name}</td>
            <td>${student.age}</td>
            <td>${student.gpa}</td>
        `;

        tableBody.appendChild(row);
    });
}

function displaySingleStudent(student) {
    const tableBody = document.getElementById('studentsBody');
    tableBody.innerHTML = ''; // Clear previous data

    const row = document.createElement('tr');

    row.innerHTML = `
            <td>${student.id}</td>
            <td>${student.name}</td>
            <td>${student.age}</td>
            <td>${student.gpa}</td>
        `;

    tableBody.appendChild(row);
}
