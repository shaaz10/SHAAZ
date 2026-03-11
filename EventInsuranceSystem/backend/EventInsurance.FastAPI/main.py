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

@app.route("/enhance-text", methods=["POST"])
def enhance_text():
    data = request.get_json()
    if not data or "text" not in data:
        return jsonify({"error": "No text provided"}), 400
    
    input_text = data["text"].lower()
    if not input_text.strip():
        return jsonify({"enhanced_text": ""})

    # Insurance Transformation Matrix
    # We simulate a more "intelligent" expansion based on keywords
    enhanced = data["text"] # Default

    if "rain" in input_text or "weather" in input_text or "cancelled" in input_text:
        enhanced = (
            "Following a critical assessment of the meteorological data, it has been determined that "
            "the scheduled event was compromised by severe inclement weather conditions (precipitating rainfall). "
            "This occurrence has triggered the 'Force Majeure' and 'Adverse Weather' indemnity clauses, "
            "necessitating a formal cancellation to ensure attendee safety and risk mitigation. "
            "Requesting immediate evaluation for loss of revenue and non-refundable deposit recovery."
        )
    elif "broken" in input_text or "damaged" in input_text or "equipment" in input_text:
        enhanced = (
            "A technical post-incident evaluation reveals that essential event infrastructure has been "
            "critically compromised due to unforeseen mechanical failure and structural damage. "
            "In accordance with the equipment floaters under policy protocols, we are submitting this "
            "narrative to initiate a comprehensive appraisal and expedited settlement for technical replacement."
        )
    elif "hurt" in input_text or "accident" in input_text or "injury" in input_text:
        enhanced = (
            "Medical reports and incident logs confirm a bodily injury occurrence involving an attendee "
            "during the primary operational phase of the event. This triggers the General Liability "
            "adjudication workflow. We have documented all clinical evidence and are submitting for "
            "administrative review to determine liability coverage and non-economic damages."
        )
    elif "theft" in input_text or "stolen" in input_text or "lost" in input_text:
        enhanced = (
            "Preliminary security investigations confirm an unauthorized breach leading to the misappropriation "
            "of insured assets. A police report has been filed and is cross-referenced here. We request "
            "an investigation into the commercial crime coverage provisions of the active policy to "
            "mitigate the resulting financial volatility."
        )
    else:
        # Generic professionalization
        prefixes = [
            "Upon thorough investigation of the circumstances, ",
            "In accordance with established policy protocols, ",
            "Based on the clinical evidence and reported incident details, ",
            "The preliminary assessment indicates that ",
            "Following a comprehensive review of the indemnity request, "
        ]
        suffixes = [
            " necessitating immediate administrative review.",
            " which aligns with our comprehensive coverage standards.",
            " to ensure optimal risk mitigation and resolution.",
            " for final adjudication and settlement processing.",
            " in adherence to our integrity and transparency guidelines."
        ]
        enhanced = random.choice(prefixes) + data["text"][0].lower() + data["text"][1:] + random.choice(suffixes)

    return jsonify({
        "original_text": data["text"],
        "enhanced_text": enhanced,
        "model": "Insurance-GPT v4 (Simulated - Optimized for Evaluation)"
    })

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=8000, debug=True)
