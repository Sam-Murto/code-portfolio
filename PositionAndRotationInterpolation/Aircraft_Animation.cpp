#include "Aircraft_Animation.h"


Aircraft_Animation::Aircraft_Animation()
{
	this->m_model_mat = glm::mat4(1.0f);
}


Aircraft_Animation::~Aircraft_Animation()
{
}

void Aircraft_Animation::init()
{
	reset();
}

void Aircraft_Animation::init(Curve* animation_curve)
{
	m_animation_curve = animation_curve;
	rotation = animation_curve->control_points_quaternion[0];
	fill_table();
	print_table();
	reset();
}

void Aircraft_Animation::update(float delta_time)
{
	//Get distance along curve
	float s = ease(current_t / total_moving_time);

	//Position handling
	int index_p = table_lookup(s);
	glm::vec4 newPosition = get_point(table(s, index_p), index_p);
	m_model_mat[3] = newPosition;

	//New Quaternion at each control point and exactly 200 curve points per control point
	int index_r0 = index_p / 200;
	int index_r1 = index_r0 * 200;
	int index_r2 = (index_r0 + 1) * 200;

	//Now has fraction u between control points
	float u = table(s, index_r1, index_r2);

	glm::quat qt = slerp(
		m_animation_curve->control_points_quaternion[index_r0],
		//If the previous control point is the final control point, slerp between final and first control points
		(index_r0 < m_animation_curve->control_points_quaternion.size() -1)?
		m_animation_curve->control_points_quaternion[index_r0 + 1]
		: m_animation_curve->control_points_quaternion[0],
		u);

	m_model_mat = rotate(qt);
	rotation = qt;

	if (current_t < total_moving_time && enabled) {
		current_t += delta_time;
	}


}

void Aircraft_Animation::reset()
{
	m_model_mat = glm::mat4(1.0f);
	if (m_animation_curve != nullptr && m_animation_curve->control_points_pos.size() > 0)
	{
		rotation = m_animation_curve->control_points_quaternion[0];
		m_model_mat = glm::toMat4(rotation) * m_model_mat;
		glm::mat4 translation = glm::mat4( {1,0,0,0},
			{0,1,0,0},
			{0,0,1,0},
			glm::vec4(m_animation_curve->control_points_pos[0], 1) );
		m_model_mat = translation * m_model_mat;
	}

	current_t = 0;
}

float Aircraft_Animation::ease(float t) {

	float s = 0;

	float v_max = (t1 / 2) + (t2 - t1) + (0.5f) * (1 - t2);
	v_max = 1 / v_max;

	if (t < t1) {
		s = v_max * t * t / (2 * t1);
	}
	else if (t1 <= t && t < t2) {
		s = v_max * (t1 / 2) + v_max * (t - t1);
	}
	else if (t2 < t) {
		s = v_max * (t1 / 2) + v_max * (t2 - t1) + v_max * (1 - ((t - t2) / (2 * (1 - t2)))) * (t - t2);
	}

	return s;

}

float Aircraft_Animation::table(float s, int i) {
	
	//Index should not be out of bounds
	if (i < 0 || i >= distances.size())
		return -1;
	
	float s1 = distances[i];
	float s2 = (i < distances.size() - 1)? distances[i+1] : 1;

	//Get fraction of segment traveled for u value
	float delta_s = s2 - s1;
	float u = s - s1;
	u = u / delta_s;

	return u;
}

float Aircraft_Animation::table(float s, int i, int j) {

	//Indeces should not be out of bounds
	if (i < 0 || i >= distances.size() || j < 0 || j > distances.size())
		return -1;
	

	float s1 = distances[i];
	float s2 = (j < distances.size())? distances[j] : 1;

	//Get fraction of segment traveled for u value
	float delta_s = s2 - s1;
	float u = s - s1;
	u = u / delta_s;

	return u;
}

glm::vec4 Aircraft_Animation::get_point(float u, int i) {
	
	glm::vec3 v1 = m_animation_curve->curve_points_pos[i];
	glm::vec3 v2 = (i < distances.size() - 1)? 
		m_animation_curve->curve_points_pos[i + 1] : m_animation_curve->curve_points_pos[0];

	glm::vec3 p = v1 + u * (v1 - v2);

	return glm::vec4(p, 1.0f);
}

int Aircraft_Animation::table_lookup(float s) {
	
	int index = 0;

	//Get index of first interpolation point
	for (int i = 0; i < distances.size(); i++) {

		if (distances[i] > s)
		{
			index = i;
			break;
		}

		if (i == distances.size() - 1) {
			index = i;
		}

	}
	return index;

}

glm::quat Aircraft_Animation::slerp(glm::quat q1, glm::quat q2, float u) {

	float omega = glm::asin(glm::dot(q1, q2));
	if (glm::degrees(omega) > 90) {
		q2 = -q2;
		omega = glm::asin(glm::dot(q1, q2));
	}

	glm::quat qt = ( glm::sin((1 - u) * omega) / glm::sin(omega) ) * q1
			+ ( glm::sin(u * omega) / glm::sin(omega) ) * q2;

	return glm::normalize(qt);
}

glm::mat4 Aircraft_Animation::rotate(glm::quat qt) {
	glm::mat4 translation = glm::mat4(
		{ 1,0,0,0 },
		{ 0,1,0,0 },
		{ 0,0,1,0 },
		m_model_mat[3]);

	return translation 
		* glm::toMat4(qt) 
		* glm::inverse(glm::toMat4(rotation)) 
		* glm::inverse(translation) 
		* m_model_mat;


}

void Aircraft_Animation::fill_table() {

	float length = 0;

	int numControlPoints = m_animation_curve->control_points_pos.size();
	int numCurvePoints = m_animation_curve->curve_points_pos.size();

	for (int i = 0; i < numCurvePoints; i++) {
		int j = i;
		distances.push_back(length);

		glm::vec3 v1 = m_animation_curve->curve_points_pos[j];
		j++;
		if (j >= numCurvePoints)
			j = 0;
		glm::vec3 v2 = m_animation_curve->curve_points_pos[j];
		length += glm::distance(v1, v2);

	}
	arc_distance = length;

	//Normalize distances
	for (int i = 0; i < numCurvePoints; i++)
	{
		distances[i] = distances[i] / arc_distance;
	}

}

void Aircraft_Animation::print_table() {
	for (int i = 0; i < distances.size(); i++) {

		glm::vec3 point = m_animation_curve->curve_points_pos[i];

		std::cout << point.x << ", " << point.y << ", " << point.z << " | " << distances[i] << '\n';

	}
}
