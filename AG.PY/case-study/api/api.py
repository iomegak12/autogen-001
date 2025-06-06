from fastapi import FastAPI
from pydantic import BaseModel

from teams import main

app = FastAPI()


class ChatRequest(BaseModel):
    query: str
    customer_id: str = "Guest"


@app.post("/chat")
async def chat(request: ChatRequest):
    """
    Endpoint to handle chat requests.
    Accepts a query and an optional customer ID.
    Returns the response from the planning agent team.
    """
    response = await main(request.query)
    return {"response": response}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000, reload=True)
