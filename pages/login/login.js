let form = document.getElementById("login-form")

// mock

// async function fetch(target, params) {
// 	return {
// 		status: 400
// 	}
// }


form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.target, {
		method: form.getAttribute("method"),
		body: new FormData(form)
	})

	//let result = await response.json()
	
	if (response.status === 200) {
		document.location.assign("https://localhost:5000/fortress/2fa")
		//document.location.assign("http://localhost:5000/2fa/index.html")
	}
	else if (response.status === 400) {
		let errorDialog = document.getElementById("login-email-error")
		
		errorDialog.show()
	}
}
