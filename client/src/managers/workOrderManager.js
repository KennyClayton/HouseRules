//^ This component handles the HTTP request functions for work orders:
  //$1 - retrieving the list of incomplete orders from the server at the route parameter "api/workorder/incomplete"
  //$2 - creating a new work order "api/workorder"
  //$3 - updating a work order
  //$4 - completing a work order 


const _apiUrl = "/api/workorder";
  
//^1 GET - this function gets a list of all incomplete work orders
export const getIncompleteWorkOrders = () => {
  return fetch(_apiUrl + "/incomplete").then((res) => res.json());
};


//^2 POST - this one creates the new order
export const createWorkOrder = (workOrder) => {
  return fetch(_apiUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(workOrder),
  }).then((res) => res.json);
};


//^3 PUT - this function updates the work order  
  //& This goes to WorkOrderList.js and updates that component to use this function
export const updateWorkOrder = (workOrder) => {
  return fetch(`${_apiUrl}/${workOrder.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(workOrder),
  });
};


//^4 PUT - this function completes a work order
export const finishedWorkOrder = (workOrder) => {
  return fetch(`${_apiUrl}/${workOrder.id}/complete`, 
  {
    method: "PUT", 
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(workOrder), //this is where the updated DateCompleted property is placed into, within this http PUT request. The PUT request is the vehicle, and the updated DateCompleted property is a passenger.
  })
};
  
//^5 DELETE - this function deletes a work order
export const deleteThisWorkOrder = (workOrder) => {
  return fetch(`${_apiUrl}/${workOrder.id}`,
  {
    method: "DELETE",
  })
};