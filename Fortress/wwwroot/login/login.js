let form = document.getElementById("login-form")

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let data = new FormData(form)

	data = Object.fromEntries(data.entries())
	data = JSON.stringify(data)
	
	let response = await fetch(form.action, {
		method: "patch",
		body: data,
		headers: { "Content-Type": "application/json", "Accept": "application/json" }
	})
	
	let { userId } = await response.json()
	
	if (response.status === 200) {
		document.cookie = `login_id=${userId}; path=/; max-age=3600`;
		document.location.assign("/2fa")
	}
	else if (response.status === 400 || response.status === 401) {
		let errorDialog = document.getElementById("login-email-error")
		
		errorDialog.show()
	}
}
