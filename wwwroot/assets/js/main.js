/*
* NOTE: I would normally use Typescript instead of plain JavaScript, this is just for simpicity.
*/

const GET = "GET";
const POST = "POST";
const DELETE = "DELETE";
async function Fetch(method, url, data = null) {
    let options = {
        method: method,
        headers: { "Content-type": "application/json", Accept: 'application/json' },
    };

    if(data !== null) {
        options = {
            method: method,
            body: JSON.stringify(data),
            headers: { "Content-type": "application/json", },
        };
    }

    try {
        let response = await fetch(url, options);
        if(response.ok) {
            let json = await response.json();
            return json;
        }
    } catch(error) {
        console.error(error);
    }

    return {};
}

async function PopulateList(list) {
    if(list === null || list === undefined) return;
    
    let rows = "";
    for(let i = 0; i < list.length; i++) {
        let status = list[i].status;
        let statusText = status === 0 ? "NotSet" : status === 1 ? "Active" : "Inactive";
        rows += `
        <tr>
            <td>${list[i].name}</td>
            <td>${list[i].rfc}</td>
            <td>${list[i].bornDate}</td>
            <td data-status="${statusText.toLowerCase()}"><span>${statusText}</span></td>
            <td><a onclick="DeleteEmployee(${list[i].id})">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3" viewBox="0 0 16 16">
                <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5ZM11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H2.506a.58.58 0 0 0-.01 0H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 0 0 0 0-1h-.995a.59.59 0 0 0-.01 0H11Zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5h9.916Zm-7.487 1a.5.5 0 0 1 .528.47l.5 8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 0 0 1 .528-.47ZM8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5Z"/>
                </svg>
            </a></td>
        </tr>`;
    }
    
    document.querySelector("table > tbody").innerHTML = rows;
}
async function LoadTable() {
    let result = await Fetch(GET, "/api/v1/employee/getemployees");
    PopulateList(result.response);
}

const IsNullOrEmpty = (str) => {
    return (str === null || str.trim().length === 0);
};

async function DeleteEmployee(id) {
    var dialog = confirm("Delete employee?");
    if (dialog) {
        let result = await Fetch(DELETE, "/api/v1/employee/rm/" + id);
        console.log(result.response);
        LoadTable();
    } 
}

document.addEventListener("DOMContentLoaded", main);
async function main() {
    LoadTable();
    
    // Add new employee
    document.querySelector("form button[type = submit]").addEventListener("click", async (event) => {
        event.preventDefault();
        
        let name = document.querySelector("form input[name = name]");
        let rfc = document.querySelector("form input[name = rfc]");
        let date = document.querySelector("form input[type = date]");
        let status = document.querySelector("form select");
        
        if(IsNullOrEmpty(name.value) || IsNullOrEmpty(rfc.value) || IsNullOrEmpty(date.value) || IsNullOrEmpty(status.value)) {
            alert("Please fill all required fields");
            return;
        }
        
        let dto = {name: name.value, rfc: rfc.value, bornDate: date.value, status: status.value};
        let result = await Fetch(POST, "/api/v1/employee/add", dto);
        if(!result.successful) {
            alert(result.errorMessage);
        } else {
            LoadTable();
            name.value = ""; rfc.value = ""; date.value = ""; status.value = 0;
        }
    });
    
    // Search by name event
    document.querySelector("#search-btn").addEventListener("click", async (event) => {
        let inputValue = document.getElementById("search-input").value;
        let result = await Fetch(GET, "/api/v1/employee/getemployees?name=" + inputValue);
        
        if(result.successful) {
            PopulateList(result.response);
        }
    });

    // Search when hit enter
    document.getElementById("search-input").addEventListener("keypress", (event) => {
        if(event.key === "Enter")
            document.getElementById("search-btn").click();
    });
}