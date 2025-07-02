function onFormSubmit(e) {
  const sheet = SpreadsheetApp.getActiveSheet();
  const headers = sheet.getRange(1, 1, 1, sheet.getLastColumn()).getValues()[0]; // Row 1 = field names
  const values = e.values;

  // Build a key-value object from the form submission
  const data = {};
  headers.forEach((title, index) => {
    data[title.trim()] = values[index];
  });

  // Extract needed fields using exact form titles
  const payload = {
        fullName: data["Full Name"],
        phoneNumber: data["Phone Number"],
        whatAppNumber: data["WhatApp Number"], 
        email: data["Email Address"],
        cakeSize: data["Cake Size"],
        flavor: data["Flavor"],
        customFlavor: data["Custom Flavor Request (optional)"],
        colourTheme: data["Colour/Theme"],
        referenceDesign: data["Reference Design"],
        fullDeliveryAddress: data["Message on Cake"], 
        messageOnCake: data["Full Delivery Address"], 
        deliveryDate: formatDate(data["Delivery Date"]), // Convert MM/DD/YYYY → DD-MM-YYYY
        deliveryTime: data["Preferred Delivery Time"],
        specialInstructions: data["Special Instructions"]
    // You can add file upload link if required
  };

  const url = "https://<your-ngrok-or-public-api-url>/api/orders/receive"; // 👈 Replace with your C# API
