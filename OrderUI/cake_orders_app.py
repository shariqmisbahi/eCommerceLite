import streamlit as st
import requests

# API Config
API_BASE = "https://localhost:7042/api/Orders"
PAGE_SIZE = 10

st.set_page_config(page_title="Cake Orders", layout="wide")
st.title("🎂 Cake Orders Dashboard")

# Session State Initialization
if 'page' not in st.session_state:
    st.session_state.page = 1
if 'show_form_for' not in st.session_state:
    st.session_state.show_form_for = None

# Pagination Controls
col_prev, col_page, col_next = st.columns([1, 2, 1])
with col_prev:
    if st.button("⏮️ Previous") and st.session_state.page > 1:
        st.session_state.page -= 1

with col_page:
    st.markdown(f"### Page {st.session_state.page}")

with col_next:
    if st.button("⏭️ Next"):
        st.session_state.page += 1

# Fetch Orders
try:
    res = requests.get(
        f"{API_BASE}?page={st.session_state.page}&pageSize={PAGE_SIZE}", verify=False
    )
    res.raise_for_status()
    result = res.json()
    orders = result['data']
    total_count = result['totalCount']
except Exception as e:
    st.error(f"❌ Failed to fetch orders: {e}")
    st.stop()

# Sort Orders by Order Received Date DESC
orders = sorted(orders, key=lambda x: x.get('updateDate', ''), reverse=False)

st.markdown("### 🧾 Orders List")
st.divider()

# Grid Headers
header_cols = st.columns([1, 3, 2.5, 2.5, 2, 4])
header_cols[0].markdown("**Order ID**")
header_cols[1].markdown("**Customer Name**")
header_cols[2].markdown("**Received Date**")
header_cols[3].markdown("**Delivery Date**")
header_cols[4].markdown("**Status**")
header_cols[5].markdown("**Actions**")

# Orders Display
for order in orders:
    with st.container():
        cols = st.columns([1, 3, 2.5, 2.5, 2, 4])
        order_id = order['id']
        from datetime import datetime

        def format_date(date_str):
            try:
                if not date_str or "0001" in date_str:
                    return "-"
                dt = datetime.fromisoformat(date_str.replace("Z", "").replace("T", " "))
                return dt.strftime("%d-%m-%Y")
            except Exception:
                return "-"

        received_date = format_date(order.get('updateDate'))
        delivery_date = format_date(order.get('deliveryDate'))

        dispatched = order['isDispatched']

        cols[0].write(f"#{order_id}")
        cols[1].write(order['fullName'])
        cols[2].write(received_date)
        cols[3].write(delivery_date)
        status = "✅ Completed" if dispatched else "⌛ Pending"
        status_color = "green" if dispatched else "orange"
        cols[4].markdown(
            f"<span style='color:{status_color}; font-weight:bold'>{status}</span>",
            unsafe_allow_html=True
        )

        with cols[5]:
            btn_col1, btn_col2 = st.columns([1, 1])
            if btn_col1.button("🧾 Invoice", key=f"invoice_{order_id}"):
                try:
                    inv_res = requests.post(
                        f"{API_BASE}/{order_id}/generate-invoice-new", verify=False
                    )
                    inv_res.raise_for_status()
                    data = inv_res.json()
                    invoice_url = data.get("invoiceUrl")

                    if invoice_url:
                        st.success(f"✅ Invoice generated for Order #{order_id}")
                        st.markdown(f"[📄 Download Invoice]({invoice_url})", unsafe_allow_html=True)
                    else:
                        st.warning("⚠️ Invoice URL not returned.")
                except Exception as e:
                    st.error(f"❌ Failed to generate invoice: {e}")

            if dispatched:
                btn_col2.button("📦 Dispatch", key=f"dispatch_{order_id}", disabled=True)
            else:
                if btn_col2.button("📦 Dispatch", key=f"dispatch_{order_id}"):
                    st.session_state.show_form_for = order_id

        # Dispatch Form
        if st.session_state.show_form_for == order_id:
            with st.form(f"dispatch_form_{order_id}", clear_on_submit=True):
                st.subheader(f"📬 Dispatch Order #{order_id}")
                tracking_url = st.text_input("Tracking URL", placeholder="https://track.courier.com/1234")
                invoice_file = st.file_uploader("Attach Invoice (.docx or .pdf)", type=["docx", "pdf"])
                submit = st.form_submit_button("✅ Confirm & Send Email")

                if submit:
                    if not tracking_url or not invoice_file:
                        st.warning("⚠️ Please provide both tracking URL and invoice.")
                    else:
                        try:
                            files = {
                                'trackingUrl': (None, tracking_url),
                                'invoiceFile': (invoice_file.name, invoice_file, invoice_file.type)
                            }
                            response = requests.post(
                                f"{API_BASE}/{order_id}/dispatch", files=files, verify=False
                            )
                            response.raise_for_status()
                            st.success(f"✅ Order #{order_id} dispatched and email sent!")
                            st.session_state.show_form_for = None
                        except Exception as e:
                            st.error(f"❌ Failed to dispatch: {e}")





# import streamlit as st
# import requests

# API_BASE = "https://localhost:7042/api/Orders"
# INVOICE_BASE = "https://localhost:7042/api/Orders"
# PAGE_SIZE = 10

# st.set_page_config(page_title="Cake Orders", layout="wide")
# st.title("🍰 Cake Orders Dashboard")

# # Pagination
# page = st.number_input("Page", min_value=1, step=1, value=1)

# try:
#     res = requests.get(f"{API_BASE}?page={page}&pageSize={PAGE_SIZE}", verify=False)
#     res.raise_for_status()
#     result = res.json()
#     orders = result['data']
#     total_count = result['totalCount']
# except Exception as e:
#     st.error(f"Failed to fetch orders: {e}")
#     st.stop()

# if 'show_form_for' not in st.session_state:
#     st.session_state.show_form_for = None

# for order in orders:
#     col1, col2, col3, col4, col5, col6, col7 = st.columns([1, 2, 2, 2, 2, 2, 4])
#     col1.write(order['id'])
#     col2.write(order['fullName'])
#     col4.write(order['deliveryDate'].split('T')[0])
#     col5.write("✅" if order['isDispatched'] else "❌")

#     if col6.button("Generate Invoice", key=f"invoice_{order['id']}"):
#         try:
#             inv_res = requests.post(f"{API_BASE}/{order['id']}/generate-invoice-new", verify=False)
#             inv_res.raise_for_status()
#             data = inv_res.json()
#             invoice_url = data.get("invoiceUrl")

#             if invoice_url:
#                 st.success(f"Invoice generated for Order #{order['id']}")
#                 st.markdown(f"[📄 Download Invoice]({invoice_url})", unsafe_allow_html=True)
#             else:
#                 st.warning("Invoice URL not returned from server.")

#         except Exception as e:
#             st.error(f"Failed to generate invoice: {e}")

#     dispatch_key = f"dispatch_{order['id']}"

#     if order['isDispatched']:
#         col7.button("Dispatch", key=dispatch_key, disabled=True)
#     else:
#         if col7.button("Dispatch", key=dispatch_key):
#             st.session_state.show_form_for = order['id']

#         if st.session_state.show_form_for == order['id']:
#             with st.form(f"dispatch_form_{order['id']}", clear_on_submit=True):
#                 st.subheader(f"📦 Dispatch Order #{order['id']}")
#                 tracking_url = st.text_input("Tracking URL", placeholder="https://track.courier.com/1234")
#                 invoice_file = st.file_uploader("Attach Invoice (.docx or .pdf)", type=["docx", "pdf"])
#                 submit_dispatch = st.form_submit_button("Send Dispatch Email")

#                 if submit_dispatch:
#                     if not tracking_url or not invoice_file:
#                         st.warning("Please provide both tracking URL and invoice file.")
#                     else:
#                         try:
#                             files = {
#                                 'trackingUrl': (None, tracking_url),
#                                 'invoiceFile': (invoice_file.name, invoice_file, invoice_file.type)
#                             }
#                             response = requests.post(
#                                 f"{API_BASE}/{order['id']}/dispatch",
#                                 files=files,
#                                 verify=False
#                             )
#                             response.raise_for_status()
#                             st.success(f"Order #{order['id']} dispatched successfully and email sent!")
#                             st.session_state.show_form_for = None  # close form
#                         except Exception as e:
#                             st.error(f"Failed to dispatch order: {e}")



# # Fetch Orders
# try:
#     res = requests.get(f"{API_BASE}?page={page}&pageSize={PAGE_SIZE}", verify=False)
#     res.raise_for_status()
#     result = res.json()
#     orders = result['data']
#     total_count = result['totalCount']
# except Exception as e:
#     st.error(f"Failed to fetch orders: {e}")
#     st.stop()

# # Display Table with Buttons
# for order in orders:
#     col1, col2, col3, col4, col5, col6, col7 = st.columns([1, 2, 2, 2, 2, 2, 4])
#     col1.write(order['id'])
#     col2.write(order['fullName'])
#     col4.write(order['deliveryDate'].split('T')[0])
#     col5.write("✅" if order['isDispatched'] else "❌")

#     if col6.button("Generate Invoice", key=f"invoice_{order['id']}"):
#         try:
#             inv_res = requests.post(f"{API_BASE}/{order['id']}/generate-invoice", verify=False)
#             inv_res.raise_for_status()
#             data = inv_res.json()
#             invoice_url = data.get("invoiceUrl")

#             if invoice_url:
#                 st.success(f"Invoice generated for Order #{order['id']}")
#                 st.markdown(f"[📄 Download Invoice]({invoice_url})", unsafe_allow_html=True)
#             else:
#                 st.warning("Invoice URL not returned from server.")

#         except Exception as e:
#             st.error(f"Failed to generate invoice: {e}")
        
#         dispatch_key = f"dispatch_{order['id']}"

#     if order['isDispatched']:
#         col7.button("Dispatch", key=f"dispatch_{order['id']}", disabled=True)
#     else:
#         if col7.button("Dispatch", key=dispatch_key):
#             st.session_state.show_form_for = order['id']

#         if st.session_state.show_form_for == order['id']:
#             with st.form(f"dispatch_form_{order['id']}", clear_on_submit=True):
#                 st.subheader(f"📦 Dispatch Order #{order['id']}")
#                 tracking_url = st.text_input("Tracking URL", placeholder="https://track.courier.com/1234")
#                 invoice_file = st.file_uploader("Attach Invoice (.docx or .pdf)", type=["docx", "pdf"])
#                 submit_dispatch = st.form_submit_button("Send Dispatch Email")

#                 if submit_dispatch:
#                     if not tracking_url or not invoice_file:
#                         st.warning("Please provide both tracking URL and invoice file.")
#                     else:
#                         try:
#                             files = {
#                                 'trackingUrl': (None, tracking_url),
#                                 'invoiceFile': (invoice_file.name, invoice_file, invoice_file.type)
#                             }
#                             response = requests.post(
#                                 f"{API_BASE}/{order['id']}/dispatch",
#                                 files=files,
#                                 verify=False
#                             )
#                             response.raise_for_status()
#                             st.success(f"Order #{order['id']} dispatched successfully and email sent!")
#                             st.session_state.show_form_for = None  # close form
#                         except Exception as e:
#                             st.error(f"Failed to dispatch order: {e}")

# # Pagination Footer
# total_pages = (total_count + PAGE_SIZE - 1) // PAGE_SIZE
# st.write(f"Page {page} of {total_pages}")

