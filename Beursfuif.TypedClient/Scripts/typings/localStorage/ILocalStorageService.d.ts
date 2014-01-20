 interface ILocalStorageService{
     isSupported: boolean
     add(key: string, value: string): void
     get(key: string): string
     remove(key: string): void
     clearAll(): void
     cookie:ILocalStorageServiceCookie;
 }

 interface ILocalStorageServiceCookie {
     add(key: string, value: string): void
     get(key: string): string
     remove(key: string): void
     clearAll(): void
 }

/*
Call	Arguments	Action	Returns
isSupported	n/a	Checks the browsers support for local storage	Boolean for success
add	key, value	Adds a key-value pair to the browser local storage	Boolean for success
get	key	Gets a value from local storage	Stored value
remove	key	Removes a key-value pair from the browser local storage	Boolean for success
clearAll	n/a	Warning Removes all local storage key-value pairs for this app.	Boolean for success
cookie	add | get | remove | clearAll	Each function within cookie uses the same arguments as the coresponding local storage functions	n/a*/