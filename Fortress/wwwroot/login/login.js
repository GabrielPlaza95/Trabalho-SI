let form = document.getElementById("login-form")

// mock

// async function fetch(target, params) {
// 	return {
// 		status: 400
// 	}
// }


form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.action, {
		method: "patch",
		body: new FormData(form),
		headers: { "Content-Type": "application/json" }
	})

	//let result = await response.json()
	
	if (response.status === 200) {
		document.location.assign("/2fa")
		//document.location.assign("http://localhost:5000/2fa/index.html")
	}
	else if (response.status === 400) {
		let errorDialog = document.getElementById("login-email-error")
		
		errorDialog.show()
	}
}
