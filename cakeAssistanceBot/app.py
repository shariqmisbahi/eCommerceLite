import streamlit as st
import google.generativeai as genai
import os
import pandas as pd
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity

# Configure API Key
# Ensure your GEMINI_API_KEY is set as an environment variable
genai.configure(api_key=os.getenv("GEMINI_API_KEY"))

# Initialize generative model
gemini_generative_model = genai.GenerativeModel("models/gemini-pro")

# Load product data
try:
    df = pd.read_csv("cakes.csv")
except FileNotFoundError:
    st.error("cakes.csv not found. Please make sure the file is in the same directory.")
    st.stop()

# Function to get embeddings
# The embedding model is usually accessed directly via genai.embed_content
def embed_text(text):
    response = genai.embed_content(
        model="models/embedding-001", # Use the correct embedding model name
        content=text,
        task_type="retrieval_document" # or "retrieval_query", depending on use case
    )
    return response["embedding"]

# Generate embeddings for descriptions if not already present
if "embedding" not in df.columns:
    st.info("Generating embeddings for product descriptions. This may take a moment...")
    df["embedding"] = df["Description"].apply(embed_text)
    # Optionally save the DataFrame with embeddings to avoid re-generating
    # df.to_csv("cakes_with_embeddings.csv", index=False)

# Match user query to embeddings
def find_most_similar(user_query, df, top_k=1):
    query_embedding = embed_text(user_query)
    # Ensure query_embedding is a list containing the embedding vector
    scores = cosine_similarity([query_embedding], list(df["embedding"]))
    top_indices = scores[0].argsort()[-top_k:][::-1]
    return df.iloc[top_indices]

# Ask Gemini with context
def ask_gemini_with_context(user_query, context):
    prompt = f"""You are a cake ordering assistant.

Here is some context from our product catalog:
{context}

Now answer this customer question:
{user_query}
"""
    try:
        response = gemini_generative_model.generate_content(prompt)
        return response.text
    except Exception as e:
        return f"An error occurred while generating content: {e}"

# Streamlit UI
st.set_page_config(page_title="Cake Assistant 🍰", page_icon="🎂")
st.title("🍰 Ask CakeBot")

if "history" not in st.session_state:
    st.session_state.history = []

user_input = st.text_input("Ask a question about cakes, delivery, or pricing:")

if user_input:
    # Save user message
    st.session_state.history.append({"role": "user", "text": user_input})

    # Find context and ask Gemini
    context_df = find_most_similar(user_input, df)
    context_text = "\n".join(context_df["Description"].tolist())

    reply = ask_gemini_with_context(user_input, context_text)

    # Save AI reply
    st.session_state.history.append({"role": "ai", "text": reply})

# Display history
for msg in reversed(st.session_state.history):
    speaker = "👤 You" if msg["role"] == "user" else "🤖 CakeBot"
    st.markdown(f"**{speaker}:** {msg['text']}")



























# import streamlit as st
# import google.generativeai as genai
# import os
# import pandas as pd
# import numpy as np
# from sklearn.metrics.pairwise import cosine_similarity

# # Configure API Key
# genai.configure(api_key=os.getenv("GEMINI_API_KEY"))

# # Load product data
# df = pd.read_csv("cakes.csv")

# # Generate embeddings for descriptions
# def embed_text(text):
#     response = genai.embed_content(
#         model="models/embedding-001",
#         content=text,
#         task_type="retrieval_document"
#     )
#     return response["embedding"]

# if "embedding" not in df.columns:
#     df["embedding"] = df["Description"].apply(embed_text)

# # Load generative model
# model = genai.GenerativeModel("models/gemma-3n-e4b-it")
# #model = genai.GenerativeModel("models/gemma-3n-e4b-it")

# # Streamlit App UI
# st.set_page_config(page_title="Cake Assistant 🍰", page_icon="🎂")
# st.title("🍰 Ask CakeBot")

# # Session history (to keep track of messages)
# if "history" not in st.session_state:
#     st.session_state.history = []

# # Input box
# user_input = st.text_input("Ask a question about cakes, delivery, or pricing:")

# # When user types something
# if user_input:
#     # Save user message
#     st.session_state.history.append({"role": "user", "text": user_input})

#     # Ask Gemini
#     try:
#         response = model.generate_content(user_input)
#         ai_reply = response.text
#     except Exception as e:
#         ai_reply = f"❌ Error: {str(e)}"

#     # Save AI reply
#     st.session_state.history.append({"role": "ai", "text": ai_reply})

# # Display conversation history
# for msg in reversed(st.session_state.history):
#     speaker = "👤 You" if msg["role"] == "user" else "🤖 CakeBot"
#     st.markdown(f"**{speaker}:** {msg['text']}")
