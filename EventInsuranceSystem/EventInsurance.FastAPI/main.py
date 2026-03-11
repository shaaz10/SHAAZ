import logging
from flask import Flask, request, jsonify
import random

app = Flask(__name__)

# Basic logging
logging.basicConfig(level=logging.INFO)

@app.route("/", methods=["GET"])
def read_root():
    return jsonify({"message": "Event Insurance AI Engine is running (Flask mode)."})

@app.route("/validate-document", methods=["POST"])
def validate_document():
    # Attempt to read the uploaded file
    file = request.files.get("file")
    if file:
        content = file.read()
        file_size_kb = len(content) / 1024
        filename = file.filename
    else:
        file_size_kb = 0
        filename = "unknown.txt"

    # Simulated AI analysis: Calculate score based on random logic for now
    confidence = random.uniform(70.0, 99.5)
    
    return jsonify({
        "confidence_score": round(confidence, 2),
        "message": f"Document '{filename}' analyzed ({file_size_kb:.2f} KB). Simulated validation complete."
    })

@app.route("/detect-fraud", methods=["POST"])
def detect_fraud():
    file = request.files.get("file")
    if file:
        content = file.read()
        filename = file.filename
    else:
        filename = "unknown.txt"
    
    # Simulated ML analysis: higher score = higher probability of fraud
    fraud_score = random.uniform(5.0, 95.0)
    fraud_flag = fraud_score >= 70.0
    
    if fraud_score < 30:
        risk_level = "Low"
    elif fraud_score < 50:
        risk_level = "Medium"
    elif fraud_score < 70:
        risk_level = "High"
    else:
        risk_level = "Critical"

    return jsonify({
        "fraud_score": round(fraud_score, 2),
        "fraud_flag": fraud_flag,
        "risk_level": risk_level,
        "message": f"Document '{filename}' analyzed for fraud anomalies."
    })

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=8000, debug=True)
