import streamlit as st
import google.generativeai as genai
import os
import pandas as pd
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity
import re

# Configure API Key
genai.configure(api_key=os.getenv("GEMINI_API_KEY"))

# Load product data
df = pd.read_csv("productEmbeddings.csv")

# Clean column names (remove brackets, trim spaces)
df.columns = df.columns.str.strip().str.replace('[\[\]]', '', regex=True)

print("CSV Columns:", df.columns.tolist())


# Convert RM price string to float
df["PriceValue"] = df["productPrice"].replace("RM", "", regex=True).astype(float)
df["WeightValue"] = df["productDimension"].str.extract(r"(\d+(?:\.\d+)?)").astype(float)

# Embedding function
def embed_text(text):
    response = genai.embed_content(
        model="models/embedding-001",
        content=text,
        task_type="retrieval_document"
    )
    return response["embedding"]

# Add embeddings
if "embedding" not in df.columns:
    df["embedding"] = df["productDescription"].apply(embed_text)

# Similarity search
def find_most_similar(user_query, df, top_k=1):
    query_embedding = embed_text(user_query)
    scores = cosine_similarity([query_embedding], list(df["embedding"]))
    top_indices = scores[0].argsort()[-top_k:][::-1]
    return df.iloc[top_indices]

# Weight extractor from user input
def extract_weight(text):
    match = re.search(r"(\d+(\.\d+)?)\s*(kg|kilogram)", text.lower())
    return float(match.group(1)) if match else None

# Build context
def build_context(row, user_weight=None):
    base_weight = row['WeightValue']
    base_price = row['PriceValue']

    if user_weight and base_weight > 0:
        new_price = (base_price / base_weight) * user_weight
        return (
            f"Name: {row['productName']}\nDescription: {row['productDescription']}\n"
            f"Base Size: {row['productDimension']}\nBase Price: {row['productPrice']}\n"
            f"Estimated Price for {user_weight}kg: RM{new_price:.2f}"
        )
    else:
        return (
            f"Name: {row['productName']}\nDescription: {row['productDescription']}\n"
            f"Size: {row['productDimension']}\nPrice: {row['productPrice']}"
        )

# Ask Gemini with context
def ask_gemini_with_context(user_query, context):
    prompt = f"""You are a cake ordering assistant.

Here is some context about our cakes:
{context}

Please answer the following user question using the context only:
{user_query}
"""
    response = genai.GenerativeModel("models/gemma-3n-e4b-it").generate_content(prompt)
    return response.text

# Streamlit UI
st.set_page_config(page_title="Cake Assistant 🍰", page_icon="🎂")
st.title("🍰 Ask CakeBot")

if "history" not in st.session_state:
    st.session_state.history = []

user_input = st.text_input("Ask a question about cakes, delivery, or pricing:")

if user_input:
    user_weight = extract_weight(user_input)
    context_df = find_most_similar(user_input, df, top_k=2)
    context_text = "\n\n".join(context_df.apply(lambda row: build_context(row, user_weight), axis=1))

    ORDER_FORM_LINK = "https://forms.gle/WVxvFhy5fAc1WkkM9"
    order_keywords = ["i want to order", "decided", "yes finalised", "final", "decided"]

    if any(keyword in user_input.lower() for keyword in order_keywords):
        reply = f"Great! 🎉 You can place your order using this form: [Click here to order]({ORDER_FORM_LINK})"
    else:
        reply = ask_gemini_with_context(user_input, context_text)

    st.session_state.history.append({"role": "user", "text": user_input})
    st.session_state.history.append({"role": "ai", "text": reply})

for msg in reversed(st.session_state.history):
    speaker = "👤 You" if msg["role"] == "user" else "🤖 CakeBot"
    st.markdown(f"**{speaker}:** {msg['text']}")
