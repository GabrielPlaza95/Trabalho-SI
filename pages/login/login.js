let form = getElementById("login-form")


form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.target, {
		method: form.method,
		body: new FormData(form)
	})

	let result = await response.json()
	
	if (result.status === 200) {
		document.location.href = "https://localhost:5000/fortress/home"
	}
	else if (result.status === 400) {
		
	}
}
