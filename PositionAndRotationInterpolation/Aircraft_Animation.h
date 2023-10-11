#pragma once

#include <vector>
#include <iostream>

 #define GLM_ENABLE_EXPERIMENTAL
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "Curve.h"

class Aircraft_Animation
{

public:
	float total_moving_time = 10;
	float current_t = 0.0f;
	float t1 = 0.1;
	float t2 = 0.7;
	glm::quat rotation;
	bool enabled = false;

private:
	glm::mat4 m_model_mat;
	Curve* m_animation_curve = nullptr;
	std::vector<float> distances;
	float arc_distance = 0;


public:
	Aircraft_Animation();
	~Aircraft_Animation();

	void init();
	void init(Curve* animation_curve);

	void update(float delta_time);

	float ease(float t);
	float table(float s, int i);
	float table(float s, int i, int j);
	glm::vec4 get_point(float s, int i);
	int table_lookup(float s);

	glm::quat slerp(glm::quat q1, glm::quat q2, float u);
	glm::mat4 rotate(glm::quat qt);

	void fill_table();
	void print_table();


	void reset();
	glm::mat4 get_model_mat() { return m_model_mat; };
};


