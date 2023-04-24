This is a basic code demo based on the requirements supplied via pdf.
There are a few observations:
1. The code could not be run as i didnt have an api to expose the data.  This was looked into by using postman.  Unfortunately I ran out of time.  This could be a future enhancement.
2. There is no database.  I used a library called CsvHelper that can help with writing CSV files.  Of course the best approach would be to use a proper database for data storage.
3. A CSV is created for each parcel id so then is logged for each event.  This would be best used in a database.  But for external application / integration a csv is not the worst approach.  but it might be better to use JSON or XML.
4. In terms of scaling I used a while loop.  That's not the best approach.  Its possible to enhance this to use some kind of queuing service that will fire an event when a new event scan is fired.
5. In terms of enhancing this app further.  I would suggest using a form of thread pools that would allow running multiple threads of the app.
