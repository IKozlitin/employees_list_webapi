var api_url = "https://localhost:7219/api/employees";

//создание строки таблицы
function row(employee) {
    const tr = document.createElement("tr"); //новая строка для сотрудника
    tr.style.verticalAlign = "middle";//вертикальное выравнивание строки таблицы
    tr.setAttribute("data-rowid", employee.id); //атрибут, из которого можно извлечь id сотрудника

    //ячейка id
    const idTd = document.createElement("td");
    idTd.append(employee.id); //контент ячейки
    tr.append(idTd); //добалвяем ячейку в строку
    
    //ячейка фио
    const nameTd = document.createElement("td");
    nameTd.append(employee.name);
    tr.append(nameTd);

    //ячейка отдела
    const departmentTd = document.createElement("td");
    departmentTd.append(employee.department);
    tr.append(departmentTd);

    //ячейка номера телефона
    const phoneTd = document.createElement("td");
    phoneTd.append(employee.phone);
    tr.append(phoneTd);

    //ячйка с кнопками действий
    const btnTd = document.createElement("td");

    const editBtn = document.createElement("button");
    editBtn.setAttribute("data-id", employee.id);
    editBtn.classList.add("btn", "btn-warning", "me-5");
    editBtn.append("Изменить"); //контент кнопки
    //слушатель клика для получения сотрудника по id
    editBtn.addEventListener("click", (e) => {
        e.preventDefault();
        GetEmployee(employee.id);
    });
    btnTd.append(editBtn);

    const removeBtn = document.createElement("button");
    removeBtn.setAttribute("data-id", employee.id);
    removeBtn.classList.add("btn", "btn-danger", "ms-5");
    removeBtn.append("Удалить");
    removeBtn.addEventListener("click", (e) => {
        e.preventDefault();
        DeleteEmployee(employee.id);
    });
    btnTd.append(removeBtn);

    tr.append(btnTd);

    return tr;
}
//получение всех сотрудников
async function GetEmployees() {
    const response = await fetch(api_url, {
        method: "GET",
        headers: { Accept: "application/json" },
    });
    //если запрос прошел успешно
    if (response.ok === true) {
        //извлекаем данные с сотрудниками из ответа
        const employees = await response.json();
        let rows = document.querySelector("tbody");
        employees.forEach((employee) => rows.append(row(employee))); //для каждого сотрудника создаем строку с данными о нем
    }
}

//получение сотрудника по id при нажатии на кнопку Изменить
async function GetEmployee(id) {
    const response = await fetch(`${api_url}/${id}`, {
        method: "GET",
        headers: { Accept: "application/json" },
    });
    if (response.ok === true) {
        const employee = await response.json();
        const form = document.forms["employeeForm"];
        form.elements["id"].value = employee.id;
        form.elements["name"].value = employee.name;
        form.elements["department"].value = employee.department;
        form.elements["phone"].value = employee.phone;
    }
}

//добавление сотрудника
async function CreateEmployee(employeeName, employeeDepartment, employeePhone) {
    const response = await fetch(api_url, {
        method: "POST",
        headers: { Accept: "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: employeeName,
            department: employeeDepartment,
            phone: parseInt(employeePhone),
        }),
    });

    if (response.ok === true) {
        const employee = await response.json();
        document.querySelector("tbody").append(row(employee));
        reset();
    }
}

//сброс формы
function reset() {
    const form = document.forms["employeeForm"];
    form.reset();
    form.elements["id"].value = 0;
}
//обработчик для кнопки сброса формы
document.querySelector("#resetBtn").addEventListener("click", (event) => {
    event.preventDefault();
    reset();
});
//обработчик отправки данных формы на сервер
document.forms["employeeForm"].addEventListener("submit", (event) => {
    event.preventDefault();
    const form = document.forms["employeeForm"];
    const id = form.elements["id"].value;
    const name = form.elements["name"].value;
    const department = form.elements["department"].value;
    const phone = form.elements["phone"].value;
    //id в скрытом поле не меняется при добавлении сотрудника
    if (id == 0) {
        CreateEmployee(name, department, phone);
    }
    //при изменении мы получаем данные сотрудника для изменения и записываем их в форму, значение в поле id заполняется данными изменяемого объекта
    else {
        EditEmployee(id, name, department, phone);
    }
});

//изменение сотрудника
async function EditEmployee(employeeId, employeeName, employeeDepartment, employeePhone) {
    const response = await fetch(api_url, {
        method: "PUT",
        headers: { Accept: "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            id: employeeId,
            name: employeeName,
            department: employeeDepartment,
            phone: parseInt(employeePhone),
        }),
    });

    if (response.ok === true) {
        const employee = await response.json();
        //находим строку по id сотрудника, заменяем на строку с новыми данными
        document
            .querySelector(`tr[data-rowid="${employee.id}"]`)
            .replaceWith(row(employee));
        reset();
    }
}

//удаление сотрудника
async function DeleteEmployee(id) {
    const response = await fetch(`${api_url}/${id}`, {
        method: "DELETE",
        headers: { Accept: "application/json" },
    });

    if (response.ok === true) {
        const employee = await response.json();
        //удаляем строку по id сотрудника
        document.querySelector(`tr[data-rowid="${employee.id}"]`).remove();
    }
}

GetEmployees();